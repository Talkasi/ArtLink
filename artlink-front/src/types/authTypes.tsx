export type UserType = 'Artist' | 'Employer' | 'Admin';

export interface LoginData {
  email: string;
  password: string;
}

export interface ArtistRegisterData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  bio?: string;
  profilePicturePath?: string;
  experience?: number;
}

export interface EmployerRegisterData {
  companyName: string;
  email: string;
  password: string;
  cpFirstName: string;
  cpLastName: string;
}