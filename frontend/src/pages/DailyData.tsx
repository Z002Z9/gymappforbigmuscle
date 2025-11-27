import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { Button, NumberInput } from "@mantine/core";

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
    const [error, setError] = useState<string | null>(null);
    const [kcalInput, setKcalInput] = useState<number | string>("");
    const [weightInput, setWeightInput] = useState<number | string>("");
    const [updating, setUpdating] = useState(false);

    const today = new Date().toISOString().split("T")[0]; 
    const API_BASE = import.meta.env.VITE_API_BASE ?? "https://localhost:7226";

    useEffect(() => {
        if (!token) return;

        const fetchDailyData = async () => {
            setError(null);

            try {
                const res = await fetch(`${API_BASE}/api/dailydata/user/me`, {
                    method: "GET",
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!res.ok) {
                    const text = await res.text();
                    throw new Error(`API hiba: ${res.status} ${text}`);
                }

                const data: UserData[] = await res.json();

                const todayData = data.find(d => d.date === today);

                if (todayData) {
                    setUserData({ ...todayData, trainedtoday: true });
                    setKcalInput("");
                    setWeightInput("");
                } else {
                    setUserData({
                        id: 0,
                        date: today,
                        weight: 0,
                        dailykcalintake: 0,
                        trainedtoday: false,
                        trainingdaytype: "",
                    });
                    setKcalInput("");
                    setWeightInput("");
                }
            } catch (err: unknown) {
                setError(err instanceof Error ? err.message : "Ismeretlen hiba történt.");
            }
        };

        fetchDailyData();
    }, [token, API_BASE]);

    const handleSaveDailyData = async () => {
        if (!token || kcalInput === "" || weightInput === "") {
            setError("Kérjük, töltse ki mindkét mezőt!");
            return;
        }

        setUpdating(true);
        setError(null);

        try {
            const kcal = Number(kcalInput);
            const weight = Number(weightInput);

            if (userData && userData.id && userData.id > 0) {
               
                const res = await fetch(`${API_BASE}/api/dailydata/${userData.id}`, {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify({
                        date: userData.date,
                        weight: weight, 
                        dailykcalintake: userData.dailykcalintake + kcal, 
                        trainingdaytype: userData.trainingdaytype || "",
                        userId: 1,
                    }),
                });

                if (!res.ok) {
                    const errorText = await res.text();
                    console.error("Backend error:", errorText);
                    throw new Error("Hiba az adatok frissítésekor");
                }

                
                setUserData({
                    ...userData,
                    weight: weight,
                    dailykcalintake: userData.dailykcalintake + kcal,
                });
                
                
                setKcalInput("");
                setWeightInput("");
            } else {
                
                const res = await fetch(`${API_BASE}/api/dailydata`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify({
                        weight: weight,
                        dailykcalintake: kcal,
                        trainingdaytype: "",
                        userId: 1,
                    }),
                });

                if (!res.ok) {
                    const errorText = await res.text();
                    console.error("Backend error:", errorText);
                    throw new Error("Hiba az adatok létrehozásakor");
                }

                const newData = await res.json();
                setUserData(newData);
                
                
                setKcalInput("");
                setWeightInput("");
            }
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : "Ismeretlen hiba történt.");
        } finally {
            setUpdating(false);
        }
    };

    if (error && !userData) return <p style={{ color: "red" }}>{error}</p>;
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
                maxWidth: "500px",
                margin: "0 auto"
            }}
        >
            <h2 style={{ textAlign: "center" }}>Mai adatok</h2>

            <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
                <strong>Jelenlegi súly:</strong> <span>{userData.weight} kg</span>
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

            <div style={{ width: "100%", marginTop: "20px", borderTop: "1px solid #444", paddingTop: "20px" }}>
                <h3>Adatok hozzáadása</h3>
                
                <div style={{ marginBottom: "15px" }}>
                    <label style={{ display: "block", marginBottom: "5px" }}>Bevitt Kalória:</label>
                    <NumberInput
                        value={kcalInput}
                        onChange={setKcalInput}
                        placeholder="Add meg a kalóriamennyiséget"
                        min={0}
                        styles={{
                            input: {
                                backgroundColor: "#1e1e1e",
                                color: "#fff",
                                border: "1px solid #444",
                            },
                        }}
                    />
                </div>

                <div style={{ marginBottom: "15px" }}>
                    <label style={{ display: "block", marginBottom: "5px" }}>Jelenlegi súly (kg):</label>
                    <NumberInput
                        value={weightInput}
                        onChange={setWeightInput}
                        placeholder="Add meg a súlyodat"
                        min={0}
                        step={1}                        
                        styles={{
                            input: {
                                backgroundColor: "#1e1e1e",
                                color: "#fff",
                                border: "1px solid #444",
                            },
                        }}
                    />
                </div>

                <Button
                    onClick={handleSaveDailyData}
                    disabled={updating || kcalInput === "" || weightInput === ""}
                    fullWidth
                    color="blue"
                >
                    {userData.id && userData.id > 0 ? "Frissítés" : "Létrehozás"}
                </Button>

                {error && <p style={{ color: "red", marginTop: "10px" }}>{error}</p>}
            </div>
        </div>
    );
};

export default DailyData;