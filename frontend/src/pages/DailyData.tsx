import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";


interface UserData {
    id?: number;
    date: string;
    weight: number;
    dailykcalintake: number;
    trainedtoday: boolean;
    trainindaytype: string;


}

const DailyData: React.FC = () => {
    const { email, token } = useContext(AuthContext);

    const [userData, setUserData] = useState<UserData>({
        id: 0,
        date: "",
        weight: 0,
        dailykcalintake: 0,
        trainedtoday: false,
        trainindaytype: "",

    });

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    useEffect(() => {
        if (!email) return;

        const fetchUserData = async () => {
            setLoading(true);
            setError(null);
            setSuccess(false);

            try {
                const api = `https://localhost:7226/api/user/ListUserByEmail/${encodeURIComponent(email)}`;
                console.log("Fetching:", api);

                const response = await fetch(api, {
                    method: "GET",
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (!response.ok) {
                    if (response.status === 404)
                        throw new Error("Nincs ilyen id !");
                    throw new Error("Hiba az adatok lekérése során.");
                }

                const data: UserData = await response.json();
                console.log("Fetched data:", data);

                setUserData(data);
            } catch (err: unknown) {
                if (err instanceof Error) setError(err.message);
                else setError("Ismeretlen hiba történt.");
            } finally {
                setLoading(false);
            }
        };

        fetchUserData();
    }, [email, token]);

   

    return (
        <div
            style={{
                display: "flex",
                justifyContent: "center",
                gap: "40px",
                padding: "20px",
            }}
        >
            <div
                style={{
                    maxWidth: "400px",
                    padding: "20px",
                    border: "1px solid #ccc",
                    borderRadius: "10px",
                    flex: "1",
                    color: "#ffffffff",
                    backgroundColor: "#212529",
                }}
            >
                <h2 style={{ textAlign: "center", marginBottom: "20px" }}>Profil szerkesztése</h2>

                {loading && <p style={{ color: "blue" }}>Betöltés...</p>}
                {error && <p style={{ color: "red" }}>{error}</p>}
                {success && <p style={{ color: "green" }}>A profil sikeresen frissítve!</p>}

                <form>



                    <div style={{ marginBottom: "15px" }}>
                        <label>Edzés típusa</label><br />
                        <input
                            type="text"
                            value={userData.trainindaytype}
                            onChange={(e) =>
                                setUserData((prev) => ({
                                    ...prev,
                                    trainindaytype: e.target.value,
                                }))
                            }
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Súly</label><br />
                        <input
                            type="number"
                            value={userData.weight}
                            onChange={(e) =>
                                setUserData((prev) => ({
                                    ...prev,
                                    weight: Number(e.target.value),
                                }))
                            }
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Napi kalória bevitel</label><br />
                        <input
                            type="number"
                            value={userData.dailykcalintake}
                            onChange={(e) =>
                                setUserData((prev) => ({
                                    ...prev,
                                    dailykcalintake: Number(e.target.value),
                                }))
                            }
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Edzettél ma?</label><br />
                        <input
                            type="checkbox"
                            checked={userData.trainedtoday}
                            onChange={(e) =>
                                setUserData((prev) => ({
                                    ...prev,
                                    trainedtoday: e.target.checked,
                                }))
                            }
                            style={{ transform: "scale(1.5)" }}
                        />
                    </div>
                   
                </form>
            </div>
        </div>
    );
};

export default DailyData;
