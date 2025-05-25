export type UserType = 'artist' | 'employer';

export interface RegisterFormValues {
  userType: UserType;
  email: string;
  password: string;
  // Общие поля
  first_name?: string;
  last_name?: string;
  // Для художника
  bio?: string;
  profile_picture_path?: string;
  experience?: number;
  // Для работодателя
  company_name?: string;
  cp_first_name?: string;
  cp_last_name?: string;
}

export interface ArtistRegisterData {
  first_name: string;
  last_name: string;
  email: string;
  password: string;
  bio?: string;
  profile_picture_path?: string;
  experience?: number;
}

export interface ArtistRegisterRequest {
  first_name: string;
  last_name: string;
  email: string;
  password_hash: string;
  bio?: string;
  profile_picture_path?: string;
  experience?: number;
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

export interface Artist {
  id: string;
  first_name: string;
  last_name: string;
  email: string;
  bio?: string;
  profilePicturePath?: string;
  experience?: number;
}