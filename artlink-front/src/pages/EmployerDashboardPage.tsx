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
import { Employer, EmployerUpdateData } from '../types/types.tsx';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import LogoutIcon from '@mui/icons-material/Logout';
import BusinessIcon from '@mui/icons-material/Business';

const EmployerDashboardPage: React.FC = () => {
  const [employer, setEmployer] = useState<Employer | null>(null);
  const [editMode, setEditMode] = useState(false);
  const [loading, setLoading] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    fetchEmployerData();
  }, []);

  const fetchEmployerData = async () => {
    try {
      setLoading(true);
      const response = await authApi.getCurrentEmployer();
      setEmployer(response.data);
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
      await authApi.deleteEmployer(employer!.id);
      handleLogout();
    } catch (error) {
      console.error('Ошибка удаления аккаунта:', error);
    } finally {
      setLoading(false);
      setOpenDeleteDialog(false);
    }
  };

  const handleSubmit = async (values: EmployerUpdateData) => {
    try {
      setLoading(true);

      const payload = {
        id: employer?.id || '',
        company_name: values.company_name || '',
        email: values.email || '',
        cp_first_name: values.cp_first_name || '',
        cp_last_name: values.cp_last_name || '',
      };
        
      await authApi.updateEmployer(employer?.id || '', payload);
      await fetchEmployerData();
      setEditMode(false);
    } catch (error) {
      console.error('Ошибка обновления данных:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading && !employer) {
    return (
      <Container maxWidth="sm" sx={{ mt: 8, textAlign: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (!employer) {
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
          <Typography variant="h4">Личный кабинет работодателя</Typography>
          <IconButton onClick={handleLogout} color="error" title="Выйти">
            <LogoutIcon />
          </IconButton>
        </Box>

        {editMode ? (
          <EditEmployerForm 
            employer={employer} 
            onCancel={() => setEditMode(false)} 
            onSubmit={handleSubmit}
            loading={loading}
          />
        ) : (
          <EmployerProfile 
            employer={employer} 
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

const EmployerProfile: React.FC<{
  employer: Employer;
  onEdit: () => void;
  onDelete: () => void;
}> = ({ employer, onEdit, onDelete }) => {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <Avatar
        sx={{ 
          width: 150, 
          height: 150, 
          mb: 3,
          backgroundColor: 'primary.main'
        }}
      >
        <BusinessIcon sx={{ fontSize: 60 }} />
      </Avatar>
      
      <Typography variant="h5" gutterBottom>
        {employer.company_name}
      </Typography>
      
      <Typography variant="body1" gutterBottom>
        <strong>Контактное лицо:</strong> {employer.cp_first_name} {employer.cp_last_name}
      </Typography>
      
      <Typography variant="body1" gutterBottom>
        <strong>Email:</strong> {employer.email}
      </Typography>

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

const EditEmployerForm: React.FC<{
  employer: Employer;
  onCancel: () => void;
  onSubmit: (values: EmployerUpdateData) => void;
  loading: boolean;
}> = ({ employer, onCancel, onSubmit, loading }) => {
  const [values, setValues] = useState<EmployerUpdateData>({ 
    company_name: employer.company_name,
    email: employer.email,
    cp_first_name: employer.cp_first_name,
    cp_last_name: employer.cp_last_name
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setValues(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(values);
  };

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mb: 3 }}>
        <Avatar
          sx={{ 
            width: 150, 
            height: 150, 
            mb: 2,
            backgroundColor: 'primary.main'
          }}
        >
          <BusinessIcon sx={{ fontSize: 60 }} />
        </Avatar>
      </Box>

      <TextField
        fullWidth
        label="Название компании"
        name="company_name"
        value={values.company_name}
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
        label="Имя контактного лица"
        name="cp_first_name"
        value={values.cp_first_name}
        onChange={handleChange}
        margin="normal"
        required
      />
      <TextField
        fullWidth
        label="Фамилия контактного лица"
        name="cp_last_name"
        value={values.cp_last_name}
        onChange={handleChange}
        margin="normal"
        required
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

export default EmployerDashboardPage;