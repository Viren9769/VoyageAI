export interface LoginRequest {
  email: string;
  password: string;
}import { AuthUser } from './auth-user';

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expires: string;
  tokenType: string;
  user: AuthUser;
}