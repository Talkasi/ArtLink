import React, { useState } from 'react';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { TextField, Button, Box, Typography, CircularProgress } from '@mui/material';
import { LoginData, UserType } from '../../types/authTypes.tsx';
import { authApi } from '../../api/authApi.tsx';

interface LoginFormProps {
  userType: UserType;
  onSuccess: () => void;
}

const LoginForm: React.FC<LoginFormProps> = ({ userType, onSuccess }) => {
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const formik = useFormik({
    initialValues: {
      email: '',
      password: '',
    },
    validationSchema: Yup.object({
      email: Yup.string().email('Некорректный email').required('Обязательное поле'),
      password: Yup.string().required('Обязательное поле'),
    }),
    onSubmit: async (values: LoginData) => {
      try {
        setLoading(true);
        setError('');
        const response = await authApi.login(userType, values);
        localStorage.setItem('token', response.data.token);
        onSuccess();
      } catch (err) {
        setError('Неверные учетные данные');
      } finally {
        setLoading(false);
      }
    },
  });

  return (
    <Box component="form" onSubmit={formik.handleSubmit} sx={{ mt: 3 }}>
      <TextField
        fullWidth
        id="email"
        name="email"
        label="Email"
        value={formik.values.email}
        onChange={formik.handleChange}
        error={formik.touched.email && Boolean(formik.errors.email)}
        helperText={formik.touched.email && formik.errors.email}
        sx={{ mb: 2 }}
      />
      <TextField
        fullWidth
        id="password"
        name="password"
        label="Пароль"
        type="password"
        value={formik.values.password}
        onChange={formik.handleChange}
        error={formik.touched.password && Boolean(formik.errors.password)}
        helperText={formik.touched.password && formik.errors.password}
        sx={{ mb: 2 }}
      />
      {error && (
        <Typography color="error" sx={{ mb: 2 }}>
          {error}
        </Typography>
      )}
      <Button
        type="submit"
        fullWidth
        variant="contained"
        disabled={loading}
        sx={{ mt: 2 }}
      >
        {loading ? <CircularProgress size={24} /> : 'Войти'}
      </Button>
    </Box>
  );
};

export default LoginForm;