import React, { useState } from 'react';
import { Container, Paper, Typography, Link } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { UserTypeSelect } from '../components/Auth/UserTypeSelect.tsx';
import RegisterForm from '../components/Auth/RegisterForm.tsx';
import { UserType } from '../types/authTypes.tsx';

const RegisterPage: React.FC = () => {
  const [userType, setUserType] = useState<UserType | null>(null);

  const handleRegisterSuccess = () => {
    window.location.href = '/login?registered=true';
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" align="center" gutterBottom>
          Регистрация
        </Typography>
        <UserTypeSelect userType={userType} onSelect={setUserType} />
        {userType && <RegisterForm userType={userType} onSuccess={handleRegisterSuccess} />}
        <Typography variant="body2" align="center" sx={{ mt: 2 }}>
          Уже есть аккаунт?{' '}
          <Link component={RouterLink} to="/login">
            Войти
          </Link>
        </Typography>
      </Paper>
    </Container>
  );
};

export default RegisterPage;