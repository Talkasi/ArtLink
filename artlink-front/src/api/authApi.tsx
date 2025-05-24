import axios from 'axios';
import * as types from '../types/authTypes';

const API_URL = 'http://localhost:5258/api';

export const authApi = {
  login: (userType: types.UserType, data: types.LoginData) => 
    axios.post(`${API_URL}/${userType}s/login`, data),
  
  registerArtist: (data: types.ArtistRegisterData) => 
    axios.post(`${API_URL}/artists/register`, data),
  
  registerEmployer: (data: types.EmployerRegisterData) => 
    axios.post(`${API_URL}/employers/register`, data),
};