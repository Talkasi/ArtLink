import React, { useState } from 'react';
import { Container, Paper, Typography, Link } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { UserTypeSelect } from '../components/Auth/UserTypeSelect.tsx';
import LoginForm from '../components/Auth/LoginForm.tsx';
import { UserType } from '../types/types.tsx';
import {getUserRoleFromToken} from '../api/api.tsx'
import { printValue } from 'yup';

const LoginPage: React.FC = () => {
  const [userType, setUserType] = useState<UserType | null>(null);

  const handleLoginSuccess = () => {
    const token = localStorage.getItem('token');
    var role
    if (token != null) {
      role = getUserRoleFromToken(token)
      printValue(role)
    }

    if (role === 'Artist') {
      window.location.href = '/artist';
    } else if (role === 'Employer') {
      window.location.href = '/employer';
    } else {
      window.location.href = '/';
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" align="center" gutterBottom>
          Вход в систему
        </Typography>
        <UserTypeSelect userType={userType} onSelect={setUserType} />
        {userType && <LoginForm userType={userType} onSuccess={handleLoginSuccess} />}
        
        <Typography variant="body2" align="center" sx={{ mt: 3 }}>
          Еще нет аккаунта?{' '}
          <Link 
            component={RouterLink} 
            to="/register" 
            sx={{ 
              textDecoration: 'none',
              '&:hover': {
                textDecoration: 'underline'
              }
            }}
          >
            Зарегистрироваться
          </Link>
        </Typography>
      </Paper>
    </Container>
  );
};

export default LoginPage;