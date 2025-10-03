/* import React from 'react';
import logo from './logo.svg';
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.tsx</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App; */
// src/App.tsx

import React, { useState } from 'react';
import './App.css'; // You can keep your existing styles
import { LoginPage } from './components/LoginPage';
import { Dashboard } from './components/Dashboard';

function App() {
  // Store the authentication token in state. It's either a string or null.
  const [authToken, setAuthToken] = useState<string | null>(null);

  // This function will be called from LoginPage on success
  const handleLoginSuccess = (token: string) => {
    setAuthToken(token);
    // In a real app, you'd likely save the token to localStorage as well
    // localStorage.setItem('authToken', token);
  };
  
  // This function logs the user out
  const handleLogout = () => {
    setAuthToken(null);
    // localStorage.removeItem('authToken');
  };

  return (
    <div className="App">
      <header className="App-header">
        {/* We use conditional rendering here */}
        {!authToken ? (
          <LoginPage onLoginSuccess={handleLoginSuccess} />
        ) : (
          <Dashboard onLogout={handleLogout} />
        )}
      </header>
    </div>
  );
}

export default App;