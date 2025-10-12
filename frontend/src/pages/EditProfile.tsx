import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { Button } from "@mantine/core";

interface UserData {
    id?: number;
    name: string;
    email?: string;
    password: string;
    age: number;
    weight: number;
    height: number;
    gender: string;
    trainingsperweek?: number;
    protein?: number;
    fat?: number;
    carbs?: number;
    injury?: number[];
    allergys?: string[];
}

const EditProfile: React.FC = () => {
    const { email, token } = useContext(AuthContext);

    const [userData, setUserData] = useState<UserData>({
        id: undefined,
        name: "",
        email: "",
        password: "",
        age: 0,
        weight: 0,
        height: 0,
        gender: "",
        trainingsperweek: 0,
        protein: 0,
        fat: 0,
        carbs: 0,
        injury: [],
        allergys: [],
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
                        throw new Error("Nincs ilyen email cím!");
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

    const handleChange = (field: keyof UserData, value: string | number | string[] | number[]) => {
        setUserData((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    //save 
    const handleUpdateUser = async () => {
        if (!userData.id) {
            setError("Nincs ilyen id-jú felhasználó");
            return;
        }

        setLoading(true);
        setError(null);
        setSuccess(false);

        try {
            const response = await fetch(
                `https://localhost:7226/api/user/EditUserByID/${userData.id}`,
                {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify(userData),
                }
            );

            if (!response.ok) {
                throw new Error("Sikertelen adatfrissítés");
            }

            setSuccess(true);
        } catch (err: unknown) {
            if (err instanceof Error) setError(err.message);
            else setError("Ismeretlen hiba történt.");
        } finally {
            setLoading(false);
        }
    };

    const [allergysInput, setAllergysInput] = useState("");

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
                }}
            >
                <h2 style={{ textAlign: "center", marginBottom: "20px" }}>Profil szerkesztése</h2>

                {loading && <p style={{ color: "blue" }}>Betöltés...</p>}
                {error && <p style={{ color: "red" }}>{error}</p>}
                {success && <p style={{ color: "green" }}>A profil sikeresen frissítve!</p>}

                <form>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Név</label><br />
                        <input
                            type="text"
                            value={userData.name}
                            onChange={(e) => handleChange("name", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Életkor</label><br />
                        <input
                            type="number"
                            value={userData.age}
                            onChange={(e) => handleChange("age", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Súly</label><br />
                        <input
                            type="number"
                            value={userData.weight}
                            onChange={(e) => handleChange("weight", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Magasság</label><br />
                        <input
                            type="number"
                            value={userData.height}
                            onChange={(e) => handleChange("height", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Nem</label><br />
                        <input
                            type="text"
                            value={userData.gender}
                            onChange={(e) => handleChange("gender", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Edzések hetente</label><br />
                        <input
                            type="number"
                            value={userData.trainingsperweek ?? 0}
                            onChange={(e) => handleChange("trainingsperweek", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                        </div>
                        <div style={{ marginBottom: "15px" }}>
                            <label>Sérülések (vesszővel elválasztva)</label><br /> 
                            <input
                                type="text" 
                                value={(userData.injury || []).join(", ")} 
                                onChange={(e) =>
                                    handleChange("injury",e.target.value.split(",").map((s) => Number(s.trim())) 
                                    )
                                }
                                style={{
                                    width: "100%",
                                    padding: "8px",
                                    borderRadius: "5px",
                                    border: "1px solid #ccc",
                                }}
                            />
                        </div>

                        <div style={{ marginBottom: "15px" }}>
                            <label>Allergiák (vesszővel elválasztva)</label><br />
                        <input
                            type="text"
                            value={allergysInput}
                            onChange={(e) => setAllergysInput(e.target.value)}
                            onBlur={() =>
                                handleChange(
                                    "allergys",
                                    allergysInput.split(",").map((s) => s.trim()).filter((s) => s.length > 0)
                                )
                            }
                            style={{
                                width: "100%",
                                padding: "8px",
                                borderRadius: "5px",
                                border: "1px solid #ccc",
                            }}
                        />
                        </div>

                    <Button type="button" onClick={handleUpdateUser} disabled={loading}>
                        {loading ? "Mentés folyamatban..." : "Mentés"}
                    </Button>
                </form>
            </div>
        </div>
    );
};

export default EditProfile;
