export interface ArtistDto {
    id: string;
    first_name: string;
    last_name: string;
    email: string;
    bio?: string;
    profile_picture_path?: string;
    experience?: number;
}

export interface RegisterArtistDto {
    first_name: string;
    last_name: string;
    email: string;
    password_hash: string;
    bio?: string;
    profile_picture_path?: string;
    experience?: number;
}

export interface LoginArtistDto {
    email: string;
    password_hash: string;
}

export interface SearchArtistDto {
    search_prompt: string;
    experience?: number;
}
