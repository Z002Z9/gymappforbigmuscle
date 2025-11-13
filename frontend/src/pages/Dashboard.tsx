import React, { useContext, useEffect, useState } from "react";
import { AuthContext } from "../context/AuthContext";
import { ResponsiveContainer,BarChart,Bar,XAxis,YAxis,CartesianGrid,Tooltip,Legend } from "recharts";
import { Loader } from "@mantine/core";

interface DailyData {
    date: string;
    dailykcalintake: number;
}

const Dashboard: React.FC = () => {
    const { email, token } = useContext(AuthContext);
    const [data, setData] = useState<DailyData[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchDailyData = async () => {
            if (!token || !email) return;

            setLoading(true);
            setError(null);
            try {
                const res = await fetch(`/api/dailydata/user/me`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!res.ok) {
                    const text = await res.text();
                    throw new Error(`API hiba: ${res.status} ${text}`);
                }

                const json = await res.json();

                setData(json);
            } catch (err) {
                setError(err instanceof Error ? err.message : String(err));
            } finally {
                setLoading(false);
            }
        };

        fetchDailyData();
    }, [email, token]);

    if (loading) return <Loader size="sm" />;
    if (error) return <div style={{ color: "red" }}>{error}</div>;
    if (!data || data.length === 0) return <div>Nincs napi adat.</div>;

    return (
        <div style={{ width: "100%", height: 300, marginTop: 20 }}>
            <ResponsiveContainer>
                <BarChart data={data} margin={{ top: 20, right: 30, left: 0, bottom: 20 }}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#333" />
                    <XAxis dataKey="date" tick={{ fill: "#fff" }} />
                    <YAxis tick={{ fill: "#fff" }} />
                    <Tooltip contentStyle={{ backgroundColor: "#1e1e1e", color: "#fff" }} />
                    <Legend wrapperStyle={{ color: "#fff" }} />
                    <Bar dataKey="dailykcalintake" fill="#4ea1f3" />
                </BarChart>
            </ResponsiveContainer>
        </div>
    );
};

export default Dashboard;
