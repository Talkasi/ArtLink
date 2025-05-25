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
  CircularProgress
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi.tsx';
import { Artist } from '../types/authTypes.tsx';

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

  const handleSubmit = async (values: Artist) => {
    try {
      setLoading(true);
      await authApi.updateArtist(values);
      setArtist(values);
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
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 4 }}>
          <Typography variant="h4">
            Личный кабинет художника
          </Typography>
          <Button 
            variant="outlined" 
            color="error"
            onClick={handleLogout}
          >
            Выйти
          </Button>
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

        {/* Диалог подтверждения удаления */}
        <Dialog
          open={openDeleteDialog}
          onClose={() => setOpenDeleteDialog(false)}
        >
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
            >
              {loading ? <CircularProgress size={24} /> : 'Удалить'}
            </Button>
          </DialogActions>
        </Dialog>
      </Paper>
    </Container>
  );
};

// Компонент для отображения профиля
const ArtistProfile: React.FC<{
  artist: Artist;
  onEdit: () => void;
  onDelete: () => void;
}> = ({ artist, onEdit, onDelete }) => {
  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        {artist.first_name} {artist.last_name}
      </Typography>
      <Typography variant="body1" gutterBottom>
        <strong>Email:</strong> {artist.email}
      </Typography>
      <Typography variant="body1" gutterBottom>
        <strong>О себе:</strong> {artist.bio}
      </Typography>
      <Typography variant="body1" gutterBottom>
        <strong>Опыт:</strong> {artist.experience} лет
      </Typography>

      <Box sx={{ mt: 4, display: 'flex', gap: 2 }}>
        <Button 
          variant="contained" 
          onClick={onEdit}
        >
          Редактировать профиль
        </Button>
        <Button 
          variant="outlined" 
          color="error"
          onClick={onDelete}
        >
          Удалить аккаунт
        </Button>
      </Box>
    </Box>
  );
};

// Компонент для редактирования профиля
const EditArtistForm: React.FC<{
  artist: Artist;
  onCancel: () => void;
  onSubmit: (values: Artist) => void;
  loading: boolean;
}> = ({ artist, onCancel, onSubmit, loading }) => {
  const [values, setValues] = useState<Artist>(artist);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setValues(prev => ({
      ...prev,
      [name]: name === 'experience' ? parseInt(value) || 0 : value
    }));
  };

  return (
    <Box component="form" onSubmit={(e) => { e.preventDefault(); onSubmit(values); }}>
      <TextField
        fullWidth
        label="Имя"
        name="first_name"
        value={values.first_name}
        onChange={handleChange}
        margin="normal"
      />
      <TextField
        fullWidth
        label="Фамилия"
        name="last_name"
        value={values.last_name}
        onChange={handleChange}
        margin="normal"
      />
      <TextField
        fullWidth
        label="Email"
        name="email"
        type="email"
        value={values.email}
        onChange={handleChange}
        margin="normal"
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
        >
          {loading ? <CircularProgress size={24} /> : 'Сохранить'}
        </Button>
      </Box>
    </Box>
  );
};

export default ArtistDashboardPage;