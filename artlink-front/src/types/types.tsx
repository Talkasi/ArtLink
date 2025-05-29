export type UserType = 'artist' | 'employer';

export interface RegisterFormValues {
  userType: UserType;
  email: string;
  password: string;
  first_name?: string | null;
  last_name?: string | null;
  bio?: string | null;
  profile_picture?: File | null;
  experience?: number | null;
  company_name?: string | null;
  cp_first_name?: string | null;
  cp_last_name?: string | null;
}

export interface Artist {
  id: string;
  first_name: string;
  last_name: string;
  email: string;
  bio?: string | null;
  profile_picture?: File | null;
  experience?: number | null;
  password: string;
}

export interface ArtistUpdateData {
  id: string;
  first_name: string;
  last_name: string;
  email: string;
  bio?: string | null;
  profile_picture?: File | string | null;
  experience?: number | null;
}

export interface ArtistRegisterData {
  first_name: string;
  last_name: string;
  email: string;
  password: string;
  bio?: string | null;
  profile_picture?: File | null;
  experience?: number | null;
}

export interface ArtistRegisterRequest {
  first_name: string;
  last_name: string;
  email: string;
  password_hash: string;
  bio?: string | null;
  profile_picture?: File | null;
  experience?: number | null;
}

export interface EmployerRegisterData {
  company_name: string;
  email: string;
  password: string;
  cp_first_name: string;
  cp_last_name: string;
}

export interface EmployerRegisterRequest {
  company_name: string;
  email: string;
  password_hash: string;
  cp_first_name: string;
  cp_last_name: string;
}

export interface LoginData {
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password_hash: string;
}

export interface Employer {
  id: string;
  company_name: string;
  email: string;
  cp_first_name: string;
  cp_last_name: string;
}

export interface EmployerUpdateData {
  company_name: string;
  email: string;
  cp_first_name: string;
  cp_last_name: string;
}