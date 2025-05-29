import React, { useState, useEffect } from 'react';
import { 
  Container, 
  Paper, 
  Typography, 
  Button, 
  Box, 
  TextField,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  CircularProgress,
  Avatar,
  IconButton
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/api.tsx';
import { Artist } from '../types/types.tsx';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import LogoutIcon from '@mui/icons-material/Logout';

interface ArtistFormValues {
  first_name: string;
  last_name: string;
  email: string;
  bio: string;
  experience: number;
  profile_picture?: File | null;
}

const ArtistDashboardPage: React.FC = () => {
  const [artist, setArtist] = useState<Artist | null>(null);
  const [editMode, setEditMode] = useState(false);
  const [loading, setLoading] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    fetchArtistData();
  }, []);

  const fetchArtistData = async () => {
    try {
      setLoading(true);
      const response = await authApi.getCurrentArtist();
      setArtist(response.data);
    } catch (error) {
      console.error('Ошибка загрузки данных:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/login');
  };

  const handleDeleteAccount = async () => {
    try {
      setLoading(true);
      await authApi.deleteArtist(artist!.id);
      handleLogout();
    } catch (error) {
      console.error('Ошибка удаления аккаунта:', error);
    } finally {
      setLoading(false);
      setOpenDeleteDialog(false);
    }
  };

  const handleSubmit = async (values: ArtistFormValues) => {
    try {
      setLoading(true);
      
      const formData = new FormData();
      formData.append('FirstName', values.first_name);
      formData.append('LastName', values.last_name);
      formData.append('Email', values.email);
      formData.append('Bio', values.bio);
      formData.append('Experience', values.experience.toString());
      
      if (values.profile_picture instanceof File) {
        formData.append('ProfilePicture', values.profile_picture);
      }

      await authApi.updateArtist(artist?.id || '', formData);
      await fetchArtistData();
      setEditMode(false);
    } catch (error) {
      console.error('Ошибка обновления данных:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading && !artist) {
    return (
      <Container maxWidth="sm" sx={{ mt: 8, textAlign: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (!artist) {
    return (
      <Container maxWidth="sm" sx={{ mt: 8, textAlign: 'center' }}>
        <Typography variant="h6">Данные не загружены</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
          <Typography variant="h4">Личный кабинет художника</Typography>
          <IconButton onClick={handleLogout} color="error" title="Выйти">
            <LogoutIcon />
          </IconButton>
        </Box>

        {editMode ? (
          <EditArtistForm 
            artist={artist} 
            onCancel={() => setEditMode(false)} 
            onSubmit={handleSubmit}
            loading={loading}
          />
        ) : (
          <ArtistProfile 
            artist={artist} 
            onEdit={() => setEditMode(true)} 
            onDelete={() => setOpenDeleteDialog(true)}
          />
        )}

        <Dialog open={openDeleteDialog} onClose={() => setOpenDeleteDialog(false)}>
          <DialogTitle>Подтвердите удаление</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Вы уверены, что хотите удалить свой аккаунт? Это действие нельзя отменить.
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenDeleteDialog(false)}>Отмена</Button>
            <Button 
              onClick={handleDeleteAccount} 
              color="error"
              disabled={loading}
              startIcon={loading ? <CircularProgress size={20} /> : <DeleteIcon />}
            >
              Удалить
            </Button>
          </DialogActions>
        </Dialog>
      </Paper>
    </Container>
  );
};

const ArtistProfile: React.FC<{
  artist: Artist;
  onEdit: () => void;
  onDelete: () => void;
}> = ({ artist, onEdit, onDelete }) => {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      {/* <Avatar
        src={artist.profile_picture ? `${process.env.REACT_APP_API_URL}${artist.profile_picture}` : '/default-avatar.png'}
        sx={{ width: 150, height: 150, mb: 3 }}
      /> */}
      <Avatar
        src={artist.profile_picture 
          ? `${process.env.REACT_APP_API_URL}${artist.profile_picture}?t=${new Date().getTime()}` 
          : '/default-avatar.png'}
      />
      <Typography variant="h5" gutterBottom>
        {artist.first_name} {artist.last_name}
      </Typography>
      <Typography variant="body1" gutterBottom>
        <strong>Email:</strong> {artist.email}
      </Typography>
      {artist.bio && (
        <Typography variant="body1" gutterBottom sx={{ maxWidth: 600, textAlign: 'center' }}>
          <strong>О себе:</strong> {artist.bio}
        </Typography>
      )}
      {artist.experience !== null && (
        <Typography variant="body1" gutterBottom>
          <strong>Опыт:</strong> {artist.experience} {artist.experience === undefined ? 'Не известно' : artist.experience === 1 ? 'год' : artist.experience < 5 ? 'года' : 'лет'}
        </Typography>
      )}

      <Box sx={{ mt: 4, display: 'flex', gap: 2 }}>
        <Button 
          variant="contained" 
          onClick={onEdit}
          startIcon={<EditIcon />}
        >
          Редактировать
        </Button>
        <Button 
          variant="outlined" 
          color="error"
          onClick={onDelete}
          startIcon={<DeleteIcon />}
        >
          Удалить аккаунт
        </Button>
      </Box>
    </Box>
  );
};

const EditArtistForm: React.FC<{
  artist: Artist;
  onCancel: () => void;
  onSubmit: (values: ArtistFormValues) => void;
  loading: boolean;
}> = ({ artist, onCancel, onSubmit, loading }) => {
  const [values, setValues] = useState<ArtistFormValues>({ 
    first_name: artist.first_name || '',
    last_name: artist.last_name || '',
    email: artist.email || '',
    bio: artist.bio || '',
    experience: artist.experience || 0,
    profile_picture: null
  });
  
  const [preview, setPreview] = useState<string | null>(
    artist.profile_picture ? `${process.env.REACT_APP_API_URL}${artist.profile_picture}` : null
  );

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setValues(prev => ({
      ...prev,
      [name]: name === 'experience' ? parseInt(value) || 0 : value
    }));
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setValues(prev => ({ ...prev, profile_picture: file }));
      
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleRemoveImage = () => {
    setValues(prev => ({ ...prev, profile_picture: null }));
    setPreview(null);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(values);
  };

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mb: 3 }}>
        <Avatar
          src={preview || (artist.profile_picture ? `${process.env.REACT_APP_API_URL}${artist.profile_picture}` : '/default-avatar.png')}
          sx={{ width: 150, height: 150, mb: 2 }}
        />
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant="outlined"
            component="label"
          >
            Выбрать фото
            <input
              type="file"
              hidden
              accept="image/*"
              onChange={handleImageChange}
            />
          </Button>
          {(preview || artist.profile_picture) && (
            <Button
              variant="outlined"
              color="error"
              onClick={handleRemoveImage}
            >
              Удалить
            </Button>
          )}
        </Box>
      </Box>

      <TextField
        fullWidth
        label="Имя"
        name="first_name"
        value={values.first_name}
        onChange={handleChange}
        margin="normal"
        required
      />
      <TextField
        fullWidth
        label="Фамилия"
        name="last_name"
        value={values.last_name}
        onChange={handleChange}
        margin="normal"
        required
      />
      <TextField
        fullWidth
        label="Email"
        name="email"
        type="email"
        value={values.email}
        onChange={handleChange}
        margin="normal"
        required
      />
      <TextField
        fullWidth
        label="О себе"
        name="bio"
        multiline
        rows={4}
        value={values.bio}
        onChange={handleChange}
        margin="normal"
      />
      <TextField
        fullWidth
        label="Опыт (в годах)"
        name="experience"
        type="number"
        value={values.experience}
        onChange={handleChange}
        margin="normal"
        inputProps={{ min: 0 }}
      />

      <Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
        <Button 
          variant="outlined" 
          onClick={onCancel}
          disabled={loading}
        >
          Отмена
        </Button>
        <Button 
          type="submit" 
          variant="contained" 
          disabled={loading}
          startIcon={loading ? <CircularProgress size={20} /> : null}
        >
          Сохранить
        </Button>
      </Box>
    </Box>
  );
};

export default ArtistDashboardPage;