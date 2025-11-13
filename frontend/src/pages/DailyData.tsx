import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";

interface UserData {
    id?: number;
    date: string;
    weight: number;
    dailykcalintake: number;
    trainedtoday: boolean;
    trainingdaytype: string;
}

const DailyData: React.FC = () => {
    const { token } = useContext(AuthContext);

    const [userData, setUserData] = useState<UserData | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const today = new Date().toISOString().split("T")[0]; // "YYYY-MM-DD"

    useEffect(() => {
        if (!token) return;

        const fetchDailyData = async () => {
            setLoading(true);
            setError(null);

            try {
                const res = await fetch(`/api/dailydata/user/me`, {
                    method: "GET",
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!res.ok) {
                    const text = await res.text();
                    throw new Error(`API hiba: ${res.status} ${text}`);
                }

                const data: UserData[] = await res.json();

                // Keresés a mai nap adatára
                const todayData = data.find(d => d.date === today);

                if (todayData) {
                    setUserData({ ...todayData, trainedtoday: true });
                } else {
                    setUserData({
                        id: 0,
                        date: today,
                        weight: 0,
                        dailykcalintake: 0,
                        trainedtoday: false,
                        trainingdaytype: "",
                    });
                }
            } catch (err: unknown) {
                setError(err instanceof Error ? err.message : "Ismeretlen hiba történt.");
            } finally {
                setLoading(false);
            }
        };

        fetchDailyData();
    }, [token]);

    if (loading) return <p style={{ color: "blue" }}>Betöltés...</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;
    if (!userData) return null;

    return (
        <div
            style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: "20px",
                padding: "20px",
                color: "#fff",
                backgroundColor: "#212529",
                borderRadius: "10px",
                maxWidth: "400px",
                margin: "0 auto"
            }}
        >
            <h2 style={{ textAlign: "center" }}>Mai adatok</h2>

            <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
                <strong>Súly:</strong> <span>{userData.weight} kg</span>
            </div>
            <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
                <strong>Napi kalória:</strong> <span>{userData.dailykcalintake} kcal</span>
            </div>
            <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
                <strong>Edzettél ma:</strong>
                <span style={{ color: userData.trainedtoday ? "green" : "red" }}>
                    {userData.trainedtoday ? "Igen" : "Nem"}
                </span>
            </div>
            <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
                <strong>Edzés típusa:</strong> <span>{userData.trainingdaytype || "-"}</span>
            </div>
        </div>
    );
};

export default DailyData;
