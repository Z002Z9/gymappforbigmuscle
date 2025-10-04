// src/components/Dashboard.tsx

import React, { useState } from 'react';
import '../CSS_files/Dashboard.css';
import { CalorieCalculator } from './CalorieCalculator';
interface DashboardProps {
    onLogout: () => void;
}


export const Dashboard: React.FC<DashboardProps> = ({ onLogout }) => {
    // Állapot a kiválasztott menüponthoz
    const [activeMenu, setActiveMenu] = useState<string>('home');

    return (
        <div>
            
            <nav>
                <button onClick={() => setActiveMenu('Home')}>Home</button>
                <button onClick={() => setActiveMenu('Calorie calculator')}>Calorie calculator</button>
                <button onClick={() => setActiveMenu('Settings')}>Settings</button>
                <button onClick={onLogout}>LogOut</button>
            </nav>

            <hr />

           
        </div>
    );
};


