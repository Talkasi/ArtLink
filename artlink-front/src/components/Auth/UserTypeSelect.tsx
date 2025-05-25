import React from 'react';
import { Button, ButtonGroup, Typography } from '@mui/material';
import { UserType } from '../../types/authTypes.tsx';

interface UserTypeSelectProps {
  userType: UserType | null;
  onSelect: (type: UserType) => void;
}

export const UserTypeSelect: React.FC<UserTypeSelectProps> = ({ userType, onSelect }) => {
  return (
    <div style={{ textAlign: 'center', margin: '20px 0' }}>
      <Typography variant="h6" gutterBottom>
        Выберите тип пользователя
      </Typography>
      <ButtonGroup variant="contained">
        <Button
          color={userType === 'artist' ? 'primary' : 'inherit'}
          onClick={() => onSelect('artist')}
        >
          Художник
        </Button>
        <Button
          color={userType === 'employer' ? 'primary' : 'inherit'}
          onClick={() => onSelect('employer')}
        >
          Работодатель
        </Button>
      </ButtonGroup>
    </div>
  );
};