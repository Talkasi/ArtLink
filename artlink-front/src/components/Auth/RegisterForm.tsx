import React, { useState } from 'react';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import {
  TextField,
  Button,
  Box,
  Typography,
  CircularProgress,
  InputAdornment,
  IconButton,
  Divider
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { UserType } from '../../types/authTypes.tsx';
import { authApi } from '../../api/authApi.tsx';

interface RegisterFormProps {
  userType: UserType;
  onSuccess: () => void;
}

const RegisterForm: React.FC<RegisterFormProps> = ({ userType, onSuccess }) => {
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const handleClickShowPassword = () => {
    setShowPassword(!showPassword);
  };

  const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
  };

  // Схема валидации для художника
  const artistValidationSchema = Yup.object().shape({
    first_name: Yup.string().required('Обязательное поле'),
    last_name: Yup.string().required('Обязательное поле'),
    email: Yup.string().email('Некорректный email').required('Обязательное поле'),
    password: Yup.string().min(6, 'Минимум 6 символов').required('Обязательное поле'),
    bio: Yup.string(),
    experience: Yup.number().min(0, 'Не может быть отрицательным'),
  });

  // Схема валидации для работодателя
  const employerValidationSchema = Yup.object().shape({
    company_name: Yup.string().required('Обязательное поле'),
    cp_first_name: Yup.string().required('Обязательное поле'),
    cp_last_name: Yup.string().required('Обязательное поле'),
    email: Yup.string().email('Некорректный email').required('Обязательное поле'),
    password: Yup.string().min(6, 'Минимум 6 символов').required('Обязательное поле'),
  });

  const validationSchema = userType === 'artist' ? artistValidationSchema : employerValidationSchema;

  const formik = useFormik({
    initialValues: userType === 'artist' ? {
      userType: 'artist',
      first_name: '',
      last_name: '',
      email: '',
      password: '',
      bio: '',
      profile_picture_path: '',
      experience: 0,
    } : {
      userType: 'employer',
      company_name: '',
      email: '',
      password: '',
      cp_first_name: '',
      cp_last_name: '',
    },
    validationSchema,
    onSubmit: async (values) => {
      try {
        setLoading(true);
        setError('');

        if (userType === 'artist') {
          await authApi.registerArtist({
            first_name: values.first_name!,
            last_name: values.last_name!,
            email: values.email,
            password: values.password,
            bio: values.bio || '',
            profile_picture_path: values.profile_picture_path || '',
            experience: values.experience || 0,
          });
        } else {
          await authApi.registerEmployer({
            company_name: values.company_name!,
            email: values.email,
            password: values.password,
            cp_first_name: values.cp_first_name!,
            cp_last_name: values.cp_last_name!,
          });
        }

        onSuccess();
      } catch (err) {
        setError('Ошибка регистрации. Возможно, email уже используется.');
      } finally {
        setLoading(false);
      }
    },
  });

  return (
    <Box component="form" onSubmit={formik.handleSubmit} sx={{ mt: 3 }}>
      <Typography variant="h6" gutterBottom>
        {userType === 'artist' ? 'Регистрация художника' : 'Регистрация работодателя'}
      </Typography>
      <Divider sx={{ mb: 3 }} />

      {userType === 'artist' ? (
        <>
          <TextField
            fullWidth
            id="first_name"
            name="first_name"
            label="Имя"
            value={formik.values.first_name}
            onChange={formik.handleChange}
            error={formik.touched.first_name && Boolean(formik.errors.first_name)}
            helperText={formik.touched.first_name && formik.errors.first_name}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            id="last_name"
            name="last_name"
            label="Фамилия"
            value={formik.values.last_name}
            onChange={formik.handleChange}
            error={formik.touched.last_name && Boolean(formik.errors.last_name)}
            helperText={formik.touched.last_name && formik.errors.last_name}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            id="bio"
            name="bio"
            label="О себе"
            multiline
            rows={3}
            value={formik.values.bio}
            onChange={formik.handleChange}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            id="experience"
            name="experience"
            label="Опыт (в годах)"
            type="number"
            value={formik.values.experience}
            onChange={formik.handleChange}
            error={formik.touched.experience && Boolean(formik.errors.experience)}
            helperText={formik.touched.experience && formik.errors.experience}
            sx={{ mb: 2 }}
          />
        </>
      ) : (
        <>
          <TextField
            fullWidth
            id="company_name"
            name="company_name"
            label="Название компании"
            value={formik.values.company_name}
            onChange={formik.handleChange}
            error={formik.touched.company_name && Boolean(formik.errors.company_name)}
            helperText={formik.touched.company_name && formik.errors.company_name}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            id="cp_first_name"
            name="cp_first_name"
            label="Имя контактного лица"
            value={formik.values.cp_first_name}
            onChange={formik.handleChange}
            error={formik.touched.cp_first_name && Boolean(formik.errors.cp_first_name)}
            helperText={formik.touched.cp_first_name && formik.errors.cp_first_name}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            id="cp_last_name"
            name="cp_last_name"
            label="Фамилия контактного лица"
            value={formik.values.cp_last_name}
            onChange={formik.handleChange}
            error={formik.touched.cp_last_name && Boolean(formik.errors.cp_last_name)}
            helperText={formik.touched.cp_last_name && formik.errors.cp_last_name}
            sx={{ mb: 2 }}
          />
        </>
      )}

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
        type={showPassword ? 'text' : 'password'}
        value={formik.values.password}
        onChange={formik.handleChange}
        error={formik.touched.password && Boolean(formik.errors.password)}
        helperText={formik.touched.password && formik.errors.password}
        sx={{ mb: 2 }}
        InputProps={{
          endAdornment: (
            <InputAdornment position="end">
              <IconButton
                aria-label="toggle password visibility"
                onClick={handleClickShowPassword}
                onMouseDown={handleMouseDownPassword}
                edge="end"
              >
                {showPassword ? <VisibilityOff /> : <Visibility />}
              </IconButton>
            </InputAdornment>
          ),
        }}
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
        {loading ? <CircularProgress size={24} /> : 'Зарегистрироваться'}
      </Button>
    </Box>
  );
};

export default RegisterForm;