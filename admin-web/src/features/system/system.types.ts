export interface AdminAuditLog {
  id: number;
  publicId: string;
  userId: string | null;
  action: string;
  entityType: string;
  entityId: string | null;
  entityPublicId: string | null;
  oldValuesJson: string | null;
  newValuesJson: string | null;
  changedPropertiesJson: string | null;
  ipAddress: string | null;
  userAgent: string | null;
  correlationId: string | null;
  occurredAt: string;
}

export interface AdminAuditLogQuery {
  userId?: string;
  action?: string;
  entityType?: string;
  entityId?: string;
  correlationId?: string;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminProductEvent {
  id: number;
  publicId: string;
  userId: string | null;
  sessionId: string | null;
  eventName: string;
  entityType: string | null;
  entityPublicId: string | null;
  propertiesJson: string | null;
  pagePath: string | null;
  referrer: string | null;
  deviceType: string | null;
  occurredAt: string;
}

export interface AdminProductEventQuery {
  userId?: string;
  sessionId?: string;
  eventName?: string;
  entityType?: string;
  deviceType?: string;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}
