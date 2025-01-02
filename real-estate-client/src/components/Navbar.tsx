import { AppBar, Toolbar, Typography, Button, Container, Box, IconButton, Menu, MenuItem, useTheme, useMediaQuery } from '@mui/material';
import { Home, Search, Add, Person, Menu as MenuIcon } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';

interface User {
  firstName: string;
  lastName: string;
}

export const Navbar = () => {
  const navigate = useNavigate();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const userData = localStorage.getItem('user');
    if (userData) {
      setUser(JSON.parse(userData));
    }
  }, []);

  useEffect(() => {
    const handleStorageChange = () => {
      const userData = localStorage.getItem('user');
      if (userData) {
        setUser(JSON.parse(userData));
      } else {
        setUser(null);
      }
    };

    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const menuItems = [
    { icon: <Home />, label: 'Home', action: () => navigate('/') },
    { icon: <Search />, label: 'Search', action: () => navigate('/search') },
    { icon: <Add />, label: 'List Property', action: () => navigate('/list-property') },
    { icon: <Person />, label: 'Profile', action: () => navigate('/profile') },
  ];

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
    navigate('/');
  };

  return (
    <AppBar 
      position="fixed" 
      sx={{ 
        backgroundColor: 'rgba(255, 255, 255, 0.95)',
        backdropFilter: 'blur(8px)',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
      }}
    >
      <Container maxWidth="lg">
        <Toolbar disableGutters>
          <Typography
            variant="h6"
            noWrap
            sx={{
              mr: 2,
              fontFamily: 'monospace',
              fontWeight: 700,
              background: 'linear-gradient(45deg, #2196f3 30%, #21cbf3 90%)',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent',
              cursor: 'pointer',
              fontSize: isMobile ? '1.2rem' : '1.5rem'
            }}
            onClick={() => navigate('/')}
          >
            REAL ESTATE
          </Typography>

          {isMobile ? (
            <>
              <Box sx={{ flexGrow: 1 }} />
              <IconButton
                size="large"
                edge="end"
                color="inherit"
                aria-label="menu"
                onClick={handleMenu}
              >
                <MenuIcon />
              </IconButton>
              <Menu
                anchorEl={anchorEl}
                open={Boolean(anchorEl)}
                onClose={handleClose}
                anchorOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                transformOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
              >
                {menuItems.map((item) => (
                  <MenuItem 
                    key={item.label} 
                    onClick={() => {
                      item.action();
                      handleClose();
                    }}
                    sx={{ gap: 1 }}
                  >
                    {item.icon}
                    {item.label}
                  </MenuItem>
                ))}
                {user ? (
                  <>
                    <MenuItem onClick={() => navigate('/list-property')}>List Property</MenuItem>
                    <MenuItem onClick={() => navigate('/profile')}>Profile</MenuItem>
                    <MenuItem onClick={handleLogout}>Logout</MenuItem>
                  </>
                ) : (
                  <>
                    <MenuItem onClick={() => navigate('/login')}>Sign In</MenuItem>
                    <MenuItem onClick={() => navigate('/register')}>Sign Up</MenuItem>
                  </>
                )}
              </Menu>
            </>
          ) : (
            <>
              <Box sx={{ flexGrow: 1, display: 'flex', gap: 2 }}>
                {menuItems.map((item) => (
                  <Button
                    key={item.label}
                    startIcon={item.icon}
                    onClick={item.action}
                    color="inherit"
                    sx={{ 
                      '&:hover': { 
                        bgcolor: 'rgba(33, 150, 243, 0.1)',
                        transform: 'translateY(-2px)',
                        transition: 'all 0.3s'
                      }
                    }}
                  >
                    {item.label}
                  </Button>
                ))}
              </Box>

              <Box sx={{ display: 'flex', gap: 1 }}>
                {user ? (
                  <>
                    <Button
                      startIcon={<Add />}
                      variant="contained"
                      onClick={() => navigate('/list-property')}
                      sx={{ 
                        borderRadius: '20px',
                        padding: '8px 24px',
                        fontWeight: 600,
                        background: 'linear-gradient(45deg, #2196f3 30%, #21cbf3 90%)',
                        boxShadow: '0 3px 5px 2px rgba(33, 203, 243, .3)',
                        '&:hover': {
                          background: 'linear-gradient(45deg, #1e88e5 30%, #00bcd4 90%)',
                          transform: 'translateY(-2px)',
                          boxShadow: '0 6px 10px 4px rgba(33, 203, 243, .3)',
                          transition: 'all 0.3s'
                        }
                      }}
                    >
                      List Property
                    </Button>
                    <Button
                      startIcon={<Person />}
                      variant="contained"
                      onClick={() => navigate('/profile')}
                      sx={{ 
                        borderRadius: '20px',
                        padding: '8px 24px',
                        fontWeight: 600,
                        background: 'linear-gradient(45deg, #2196f3 30%, #21cbf3 90%)',
                        boxShadow: '0 3px 5px 2px rgba(33, 203, 243, .3)',
                        '&:hover': {
                          background: 'linear-gradient(45deg, #1e88e5 30%, #00bcd4 90%)',
                          transform: 'translateY(-2px)',
                          boxShadow: '0 6px 10px 4px rgba(33, 203, 243, .3)',
                          transition: 'all 0.3s'
                        }
                      }}
                    >
                      Profile
                    </Button>
                    <Button
                      variant="outlined"
                      onClick={handleLogout}
                      sx={{ 
                        borderRadius: '20px',
                        padding: '8px 24px',
                        fontWeight: 600,
                        border: '2px solid #2196f3',
                        color: '#2196f3',
                        '&:hover': { 
                          bgcolor: 'rgba(33, 150, 243, 0.1)',
                          transform: 'translateY(-2px)',
                          transition: 'all 0.3s',
                          border: '2px solid #21cbf3'
                        }
                      }}
                    >
                      Logout
                    </Button>
                  </>
                ) : (
                  <>
                    <Button
                      color="primary"
                      onClick={() => navigate('/login')}
                      sx={{ 
                        borderRadius: '20px',
                        padding: '8px 24px',
                        fontWeight: 600,
                        border: '2px solid #2196f3',
                        color: '#2196f3',
                        '&:hover': { 
                          bgcolor: 'rgba(33, 150, 243, 0.1)',
                          transform: 'translateY(-2px)',
                          transition: 'all 0.3s',
                          border: '2px solid #21cbf3'
                        }
                      }}
                    >
                      Sign In
                    </Button>
                    <Button
                      variant="contained"
                      onClick={() => navigate('/register')}
                      sx={{ 
                        borderRadius: '20px',
                        padding: '8px 24px',
                        fontWeight: 600,
                        background: 'linear-gradient(45deg, #2196f3 30%, #21cbf3 90%)',
                        boxShadow: '0 3px 5px 2px rgba(33, 203, 243, .3)',
                        '&:hover': {
                          background: 'linear-gradient(45deg, #1e88e5 30%, #00bcd4 90%)',
                          transform: 'translateY(-2px)',
                          boxShadow: '0 6px 10px 4px rgba(33, 203, 243, .3)',
                          transition: 'all 0.3s'
                        }
                      }}
                    >
                      Sign Up
                    </Button>
                  </>
                )}
              </Box>
            </>
          )}
        </Toolbar>
      </Container>
    </AppBar>
  );
};
