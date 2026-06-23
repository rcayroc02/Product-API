
export interface AppError {
  status: number;
  message: string;
  errors: string[] | null;
  timestamp: string;
}