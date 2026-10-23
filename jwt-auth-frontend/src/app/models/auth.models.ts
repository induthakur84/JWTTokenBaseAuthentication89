// DTOs matching the .NET Backend API

export interface LoginRequest {
  username: string;
  password: string;
}

export interface UserResponse {
  id?: number;
  firstName: string;
  lastName: string;
  username: string;
  role: string;
}

export interface LoginResponse {
  message: string;
  token: string;
  user: UserResponse;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  username: string;
  password: string;
  confirmPassword: string;
  role: string; // 'Admin' | 'Employee' | 'User'
}

// Decoded JWT Payload representation for educational inspection
export interface JwtDecodedToken {
  header: {
    alg: string;
    typ: string;
  };
  payload: {
    // Standard ClaimTypes in .NET:
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
    unique_name?: string;
    role?: string;
    FirstName?: string;
    LastName?: string;
    iss?: string;
    aud?: string;
    exp?: number;
    nbf?: number;
    [key: string]: any;
  };
  signature: string;
}
