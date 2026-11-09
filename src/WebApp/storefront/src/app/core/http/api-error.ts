import { HttpErrorResponse } from '@angular/common/http';

export interface ValidationError {
  propertyName: string;
  errorMessage: string;
}

// Shape written by BuildingBlocks' CustomExceptionHandler.
interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  instance?: string;
  traceId?: string;
  ValidationErrors?: ValidationError[];
  validationErrors?: ValidationError[];
}

export class ApiError extends Error {
  constructor(
    message: string,
    readonly status: number,
    readonly title?: string,
    readonly validationErrors: ValidationError[] = [],
    readonly traceId?: string,
  ) {
    super(message);
    this.name = 'ApiError';
  }

  static from(response: HttpErrorResponse): ApiError {
    if (response.status === 0) {
      return new ApiError('Cannot reach the server. Check your connection and try again.', 0);
    }

    const body = isProblemDetails(response.error) ? response.error : {};
    const validationErrors = body.ValidationErrors ?? body.validationErrors ?? [];

    return new ApiError(
      body.detail ?? defaultMessage(response.status),
      response.status,
      body.title,
      validationErrors,
      body.traceId,
    );
  }
}

function isProblemDetails(value: unknown): value is ProblemDetails {
  return typeof value === 'object' && value !== null;
}

function defaultMessage(status: number): string {
  if (status === 404) return 'The requested item was not found.';
  if (status === 429) return 'Too many requests. Please wait a moment and try again.';
  if (status >= 500) return 'Something went wrong on our side. Please try again later.';
  return 'The request could not be completed.';
}
