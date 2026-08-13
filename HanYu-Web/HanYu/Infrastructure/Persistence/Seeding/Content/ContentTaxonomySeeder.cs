using HanYu.Domain.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HanYu.Infrastructure.Persistence.Seeding.Content;

/// <summary>
/// Development seed for the shared Topic taxonomy used by Vocabulary and Lesson.
/// Idempotent by slug so restarting the API never creates duplicate topics.
/// </summary>
public sealed class ContentTaxonomySeeder
{
    private readonly HanYuDbContext _db;
    private readonly ILogger<ContentTaxonomySeeder> _logger;

    public ContentTaxonomySeeder(
        HanYuDbContext db,
        ILogger<ContentTaxonomySeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var existingSlugs = await _db.Set<Topic>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Select(x => x.Slug)
            .ToHashSetAsync(cancellationToken);

        var definitions = new[]
        {
            new TopicDefinition("chao-hoi", "Chào hỏi", "Từ vựng và bài học dùng trong chào hỏi, giới thiệu và giao tiếp cơ bản.", 10),
            new TopicDefinition("gia-dinh", "Gia đình", "Thành viên gia đình, quan hệ họ hàng và sinh hoạt gia đình.", 20),
            new TopicDefinition("con-nguoi", "Con người", "Thông tin cá nhân, ngoại hình, tính cách và các mối quan hệ xã hội.", 30),
            new TopicDefinition("so-dem", "Số đếm", "Số đếm, số thứ tự, lượng từ và cách biểu đạt số lượng.", 40),
            new TopicDefinition("thoi-gian", "Thời gian", "Giờ, ngày, tháng, năm, lịch và các biểu đạt thời gian thường dùng.", 50),
            new TopicDefinition("truong-hoc", "Trường học", "Lớp học, môn học, giáo viên, học sinh và hoạt động học tập.", 60),
            new TopicDefinition("cong-viec", "Công việc", "Nghề nghiệp, nơi làm việc, công việc hằng ngày và giao tiếp công sở.", 70),
            new TopicDefinition("an-uong", "Ăn uống", "Món ăn, đồ uống, nhà hàng, gọi món và khẩu vị.", 80),
            new TopicDefinition("mua-sam", "Mua sắm", "Giá cả, tiền tệ, cửa hàng, quần áo và giao dịch mua bán.", 90),
            new TopicDefinition("nha-cua", "Nhà cửa", "Phòng, đồ dùng gia đình, thuê nhà và sinh hoạt tại nhà.", 100),
            new TopicDefinition("giao-thong", "Giao thông", "Phương tiện, đường đi, chỉ đường và di chuyển trong thành phố.", 110),
            new TopicDefinition("du-lich", "Du lịch", "Khách sạn, tham quan, đặt chỗ, hành lý và trải nghiệm du lịch.", 120),
            new TopicDefinition("suc-khoe", "Sức khỏe", "Cơ thể, bệnh thường gặp, bệnh viện, thuốc và chăm sóc sức khỏe.", 130),
            new TopicDefinition("thoi-tiet", "Thời tiết", "Nắng, mưa, nhiệt độ, mùa và các hiện tượng thời tiết.", 140),
            new TopicDefinition("thien-nhien", "Thiên nhiên", "Động vật, thực vật, địa hình, môi trường và thế giới tự nhiên.", 150),
            new TopicDefinition("so-thich", "Sở thích", "Thể thao, âm nhạc, phim ảnh, đọc sách và hoạt động giải trí.", 160),
            new TopicDefinition("cam-xuc", "Cảm xúc", "Cảm xúc, trạng thái tinh thần, thái độ và cách bày tỏ ý kiến.", 170),
            new TopicDefinition("xa-hoi", "Xã hội", "Cộng đồng, văn hóa, sự kiện và những tình huống xã hội thường gặp.", 180),
            new TopicDefinition("cong-nghe", "Công nghệ", "Máy tính, điện thoại, Internet và các hoạt động công nghệ cơ bản.", 190),
            new TopicDefinition("ngon-ngu", "Ngôn ngữ", "Học ngoại ngữ, phát âm, chữ Hán, ngữ pháp và giao tiếp ngôn ngữ.", 200)
        };

        var added = 0;

        foreach (var definition in definitions)
        {
            if (existingSlugs.Contains(definition.Slug))
            {
                continue;
            }

            var topic = new Topic(
                definition.Slug,
                definition.NameVi,
                definition.DescriptionVi,
                definition.SortOrder);

            // Default taxonomy is immediately usable by Lesson/Vocabulary selectors,
            // both of which prefer/require Published topics for publish workflows.
            topic.Publish();

            _db.Add(topic);
            existingSlugs.Add(definition.Slug);
            added++;
        }

        if (added == 0)
        {
            _logger.LogInformation("Shared Topic seed is already up to date.");
            return;
        }

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Seeded {TopicCount} shared content topics.", added);
    }

    private sealed record TopicDefinition(
        string Slug,
        string NameVi,
        string DescriptionVi,
        int SortOrder);
}
