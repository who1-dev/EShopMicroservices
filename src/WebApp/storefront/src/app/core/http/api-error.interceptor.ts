import { HttpErrorResponse, HttpInterceptorFn, HttpRequest, HttpStatusCode } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiError } from './api-error';

// Turns every failed HTTP call into an ApiError so callers get one consistent error type.
export const apiErrorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        return throwError(() => error);
      }

      const apiError = ApiError.from(error);
      handleError(req, apiError);
      return throwError(() => apiError);
    }),
  );

// Covers the status codes the backend can produce: CustomExceptionHandler (400/404/500)
// and the YARP gateway (502/503/504).
function handleError(req: HttpRequest<unknown>, error: ApiError): void {
  const context = `[${req.method} ${req.urlWithParams}] ${error.status}`;

  switch (error.status) {
    // Network failure, CORS rejection, or the gateway is down.
    case 0:
      console.error(`${context} Network error`, error);
      break;

    // ValidationException or BadRequestException.
    case HttpStatusCode.BadRequest:
      if (error.validationErrors.length > 0) {
        console.warn(`${context} Validation failed`, error.validationErrors);
      } else {
        console.warn(`${context} Bad request`, error);
      }
      break;

    // NotFoundException.
    case HttpStatusCode.NotFound:
      console.warn(`${context} Not found`, error);
      break;

    // InternalServerException or any unhandled exception.
    case HttpStatusCode.InternalServerError:
      console.error(`${context} Internal server error`, error);
      break;

    // Gateway could not reach the downstream service.
    case HttpStatusCode.BadGateway:
      console.error(`${context} Bad gateway`, error);
      break;

    // Gateway rate limiter rejection (ASP.NET Core default) or service unavailable.
    case HttpStatusCode.ServiceUnavailable:
      console.error(`${context} Service unavailable`, error);
      break;

    // Downstream service did not respond in time.
    case HttpStatusCode.GatewayTimeout:
      console.error(`${context} Gateway timeout`, error);
      break;

    default:
      console.error(`${context} Unexpected error`, error);
      break;
  }
}
