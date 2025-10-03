// src/components/LoginPage.tsx

import React, { useState } from 'react';

// Define the type for the props this component will receive
interface LoginPageProps {
  onLoginSuccess: (token: string) => void;
}

export function LoginPage({ onLoginSuccess }: LoginPageProps) {
  // State for email, password, and any error messages
  const [email, setEmail] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [error, setError] = useState<string>('');

  // Handle the form submission
  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault(); // Prevent the browser from reloading the page

    try {
      // Send a request to your backend's login endpoint
      const response = await fetch('https://localhost:7226/api/login', { // <-- IMPORTANT: Change this to your actual API endpoint!
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        // If the server responds with an error, display it
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to log in');
      }

      const data = await response.json();
      
      // Assuming your backend returns a token upon successful login
      onLoginSuccess(data.token);

    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div>
      <h2>Login</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Email: </label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Password: </label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        {error && <p style={{ color: 'red' }}>{error}</p>}
        <button type="submit">Log In</button>
      </form>
    </div>
  );
}