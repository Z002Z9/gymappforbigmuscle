// src/components/Dashboard.tsx

import React from 'react';

interface DashboardProps {
    onLogout: () => void;
}

export function Dashboard({ onLogout }: DashboardProps) {
    return (
        <div>
            <h1>Welcome to Your Dashboard! ✨</h1>
            <p>You are successfully logged in.</p>
            <button onClick={onLogout}>Log Out</button>
        </div>
    );
}