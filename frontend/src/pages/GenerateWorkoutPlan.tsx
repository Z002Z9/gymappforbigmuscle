import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { Button } from "@mantine/core";


const GenerateWorkoutPlan: React.FC = () => {
    const { email, token } = useContext(AuthContext);

    const [userData, setUserData] = useState<UserData>({
        weight: 0,
        height: 0,
        age: 0,
        gender: "",
        protein: 0,
        fat: 0,
        carbs: 0,
        trainingsperweek: 0,
        goal: ""
    });

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [result, setResult] = useState<number | null>(null);



            return (
                <>
                </>
            );
};



export default GenerateWorkoutPlan;