import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AppError } from '../models/app-error.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((httpError: HttpErrorResponse) => {
      const normalized = normalizeError(httpError);
      console.error('[HTTP ERROR]', normalized);
      return throwError(() => normalized);
    })
  );
};

function normalizeError(httpError: HttpErrorResponse): AppError {
  const body = httpError.error;

  // Caso 1: error de red / backend caído / CORS — no hay body JSON
  if (!body || typeof body !== 'object') {
    return {
      status: httpError.status || 0,
      message: httpError.status === 0
        ? 'No se pudo conectar con el servidor. Verifica que el backend esté corriendo.'
        : 'Ocurrió un error inesperado.',
      errors: null,
      timestamp: new Date().toISOString()
    };
  }

  // Caso 2: shape de ApiResponse.Fail -> { success: false, message, data: null, errors }
  if ('success' in body) {
    return {
      status: httpError.status,
      message: body.message ?? 'Error en la operación.',
      errors: body.errors ?? null,
      timestamp: new Date().toISOString()
    };
  }

  // Caso 3: shape del InvalidModelStateResponseFactory o del ErrorHandlingMiddleware
  // -> { status, message, errors?, timestamp }
  if ('message' in body) {
    return {
      status: body.status ?? httpError.status,
      message: body.message,
      errors: body.errors ?? null,
      timestamp: body.timestamp ?? new Date().toISOString()
    };
  }

  // Fallback por si el backend devuelve algo no contemplado
  return {
    status: httpError.status,
    message: 'Ocurrió un error inesperado.',
    errors: null,
    timestamp: new Date().toISOString()
  };
}