import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import LoginPage from './pages/LoginPage.tsx';
import RegisterPage from './pages/RegisterPage.tsx';
import ArtistDashboardPage from './pages/ArtistDashboardPage.tsx';
import EmployerDashboardPage from './pages/EmployerDashboardPage.tsx';

const theme = createTheme({
  palette: {
    primary: {
      main: '#3f51b5',
    },
    secondary: {
      main: '#f50057',
    },
  },
});

const App: React.FC = () => {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/artist" element={<ArtistDashboardPage />} />
          <Route path="/employer" element={<EmployerDashboardPage />} />
        </Routes>
      </Router>
    </ThemeProvider>
  );
};

export default App;