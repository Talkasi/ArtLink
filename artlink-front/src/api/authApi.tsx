import axios from 'axios';
import * as types from '../types/authTypes';
import {jwtDecode} from 'jwt-decode';

const API_URL = 'http://localhost:7000/api';

async function sha256(message: string): Promise<string> {
  const msgBuffer = new TextEncoder().encode(message);
  const hashBuffer = await crypto.subtle.digest('SHA-256', msgBuffer);
  const hashArray = Array.from(new Uint8Array(hashBuffer));
  return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
}

export const getUserIdFromToken = (token: string): string | null => {
  try {
    const decoded = jwtDecode<{ sub: string }>(token);
    return decoded.sub;
  } catch (e) {
    console.error('Error decoding JWT:', e);
    return null;
  }
};

export const authApi = {
  login: async (userType: types.UserType, data: types.LoginData) => {
    const hashedData = {
      email: data.email,
      password_hash: await sha256(data.password),
      password: undefined
    };
    delete hashedData.password;
    return axios.post(`${API_URL}/${userType}s/login`, hashedData);
  },
  registerArtist: async (data: types.ArtistRegisterData) => {
    const hashedData = {
      ...data,
      password_hash: await sha256(data.password),
      password: undefined
    };
    delete hashedData.password;
    return axios.post(`${API_URL}/artists/register`, hashedData);
  },
  
  registerEmployer: async (data: types.EmployerRegisterData) => {
    const hashedData = {
      ...data,
      password_hash: await sha256(data.password),
      password: undefined
    };
    delete hashedData.password;
    return axios.post(`${API_URL}/employers/register`, hashedData);
  },

  getCurrentArtist: () => {
    const token = localStorage.getItem('token');
    var id
    if (token != null) {
      id = getUserIdFromToken(token)
    }
    return axios.get(`${API_URL}/artists/${id}`);
  },

  getArtistById: (id: string) => {
    return axios.get(`${API_URL}/artists/${id}`);
  },
  
  updateArtist: (data: types.Artist) => {
    const token = localStorage.getItem('token');

    const updateData = {
      ...data,
      id: undefined
    };
    
    return axios.put(`${API_URL}/artists/${data.id}`, updateData, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
  },
  
  deleteArtist: (id: string) => {
    const token = localStorage.getItem('token');
    console.log(token);

    return axios.delete(`${API_URL}/artists/${id}`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
  },
};