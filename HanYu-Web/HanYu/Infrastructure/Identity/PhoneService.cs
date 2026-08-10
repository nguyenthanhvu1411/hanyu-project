using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Phone;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Messaging;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HanYu.Infrastructure.Identity;

public sealed class PhoneService : IPhoneService
{
    private readonly UserManager<User> _userManager;
    private readonly ISmsService _smsService;

    public PhoneService(
        UserManager<User> userManager,
        ISmsService smsService)
    {
        _userManager = userManager;
        _smsService = smsService;
    }

    public async Task<Result<PhoneResponse>> UpdateAsync(
        Guid userId,
        UpdatePhoneNumberRequest request,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
            return UserNotFound();

        if (!await _userManager.CheckPasswordAsync(
                user,
                request.Password))
        {
            return Result.Failure<PhoneResponse>(
                Error.Unauthorized(
                    "Identity.InvalidPassword",
                    "Mật khẩu không chính xác."));
        }

        try
        {
            user.UpdatePhoneNumber(
                request.PhoneNumber);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<PhoneResponse>(
                Error.Validation(
                    "Identity.InvalidPhoneNumber",
                    exception.Message));
        }

        var result =
            await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure<PhoneResponse>(
                Error.Validation(
                    "Identity.InvalidPhoneNumber",
                    string.Join(
                        "; ",
                        result.Errors.Select(
                            x => x.Description))));
        }

        return Result.Success(Map(user));
    }

    public async Task<Result> SendVerificationAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (string.IsNullOrWhiteSpace(
                user.PhoneNumber))
        {
            return Result.Failure(
                Error.Validation(
                    "Identity.PhoneNumberMissing",
                    "Tài khoản chưa có số điện thoại."));
        }

        if (user.PhoneNumberConfirmed)
        {
            return Result.Success();
        }

        var code =
            await _userManager
                .GenerateChangePhoneNumberTokenAsync(
                    user,
                    user.PhoneNumber);

        await _smsService.SendVerificationCodeAsync(
            user.PhoneNumber,
            code,
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PhoneResponse>> VerifyAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
        {
            return UserNotFound();
        }

        if (string.IsNullOrWhiteSpace(
                user.PhoneNumber))
        {
            return Result.Failure<PhoneResponse>(
                Error.Validation(
                    "Identity.PhoneNumberMissing",
                    "Tài khoản chưa có số điện thoại."));
        }

        if (user.PhoneNumberConfirmed)
        {
            return Result.Success(Map(user));
        }

        var valid =
            await _userManager
                .VerifyChangePhoneNumberTokenAsync(
                    user,
                    code.Trim(),
                    user.PhoneNumber);

        if (!valid)
        {
            return Result.Failure<PhoneResponse>(
                Error.Validation(
                    "Identity.InvalidPhoneVerificationCode",
                    "Mã xác minh không hợp lệ hoặc đã hết hạn."));
        }

        user.ConfirmPhoneNumber();

        var update =
            await _userManager.UpdateAsync(user);

        if (!update.Succeeded)
        {
            return Result.Failure<PhoneResponse>(
                Error.Validation(
                    "Identity.PhoneVerificationFailed",
                    string.Join(
                        "; ",
                        update.Errors.Select(
                            x => x.Description))));
        }

        return Result.Success(Map(user));
    }

    private static PhoneResponse Map(User user) =>
        new(
            user.PhoneNumber,
            user.PhoneNumberConfirmed);

    private static Result<PhoneResponse>
        UserNotFound()
    {
        return Result.Failure<PhoneResponse>(
            Error.NotFound(
                "Identity.UserNotFound",
                "Không tìm thấy người dùng."));
    }
}
