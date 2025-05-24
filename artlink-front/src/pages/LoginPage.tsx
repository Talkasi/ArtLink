import React, { useState } from 'react';
import { Container, Paper, Typography } from '@mui/material';
import { UserTypeSelect } from '../components/Auth/UserTypeSelect.tsx';
import LoginForm from '../components/Auth/LoginForm.tsx';
import { UserType } from '../types/authTypes.tsx';

const LoginPage: React.FC = () => {
  const [userType, setUserType] = useState<UserType | null>(null);

  const handleLoginSuccess = () => {
    window.location.href = '/';
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" align="center" gutterBottom>
          Вход в систему
        </Typography>
        <UserTypeSelect userType={userType} onSelect={setUserType} />
        {userType && <LoginForm userType={userType} onSuccess={handleLoginSuccess} />}
      </Paper>
    </Container>
  );
};

export default LoginPage;