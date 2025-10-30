import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { Button } from "@mantine/core";

interface UserData {
    weight: number;
    height: number;
    age: number;
    gender: string;
    protein?: number;
    fat?: number;
    carbs?: number;
    trainingsperweek?: number;
    goal: string;
}

const Kcalcalculator: React.FC = () => {
    const { email, token } = useContext(AuthContext); // contextből vesszük az értékeket

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

    //kivesszük az emailt és meghívjuk rá végpontot hogy lekérdezzük az adatait a felhasználónak
    useEffect(() => {
        console.log("EMAIL VALUE:", email);
        console.log("Goal: ", userData.goal)
        if (!email) return;

        const fetchUserData = async () => {
            setLoading(true);
            setError(null);

            try {
               const api = `https://localhost:7226/api/user/ListUserByEmail/${encodeURIComponent(email)}`;

                console.log("Fetching:", api);

                const response = await fetch(api);

                if (!response.ok) {
                    if (response.status === 404)
                        throw new Error("Nem található felhasználó ezzel az email címmel.");
                    throw new Error("Hiba történt az API lekérés közben.");
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


    const handleChange = (field: keyof UserData, value: string | number) => {
        setUserData((prev) => ({
            ...prev,
            [field]:
                typeof value === "string" && !isNaN(Number(value)) ? Number(value) : value,

        }));
    };

 

    //kalória kiszámolása
    const handleCalculate = async () => {
        setLoading(true);
        setError(null);

        try {
            const response = await fetch("https://localhost:7226/api/Kcalcalculator/daily-calories", {
                method: "GET", 
                headers: {
                    "Authorization": `Bearer ${token}`, //csak akkor kell, ha rolehoz kötjük majd a végpontot
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error("Hiba történt az API hívás során.");
            }

            
            const data = await response.json();
            console.log("API válasz:", data);

            console.log("Fetched kcalintake:", data.dailyCalories);
            setResult(data.dailyCalories);


            console.log("Beállított result:", data.dailyCalories);
        } catch (err: unknown) {
            if (err instanceof Error) setError(err.message);
            else setError("Ismeretlen hiba történt.");
        } finally {
            setLoading(false);
        }
    };


    // tápanyagok kiszámolása
    const handleFetchMacros = async () => {
        console.log("handleFetchMacros called");
        setLoading(true);
        setError(null);

        try {
            const response = await fetch("https://localhost:7226/api/Kcalcalculator/dailymacrosget", {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json",
                },
            });

            console.log("Response status:", response.status);

            if (!response.ok) throw new Error("Hiba a makrók lekérése során");

            const data = await response.json();
            console.log("API data:", data);

            // hogy ne legyen undefined hiba
            const macros = data?.dailyMacros ?? [0, 0, 0]; 


            const [protein, fat, carbs] = macros;
            console.log(`protein: ${protein}, fat: ${fat}, carbs: ${carbs}`);

            setUserData(prev => {
                const newState = { ...prev, protein, fat, carbs };
                console.log("userData state:", newState);
                return newState;
            });

        } catch (err: unknown) {
            if (err instanceof Error) {
                console.error("Error :", err.message);
                setError(err.message);
            } else {
                console.error("Unknown error:", err);
                setError("Ismeretlen hiba történt");
            }
        } finally {
            setLoading(false);
            console.log("Loading finished");
        }
    };

    const handleCalculateAndFetchMacros = async () => {        
        await handleCalculate();      
        await handleFetchMacros();   
    };


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
                    backgroundColor: "#212529",
                    color: "#ffffffff",
                }}
            >
                <h2 style={{ textAlign: "center", marginBottom: "20px" }}>Felhasználói adatok</h2>

                {loading && <p style={{ color: "blue" }}>Betöltés...</p>}
                {error && <p style={{ color: "red" }}>⚠️ {error}</p>}

                <form>
                    <div style={{ marginBottom: "15px" }}>
                        <label>Cél</label><br />
                        <input
                            readOnly 
                           
                            value={userData.goal || ""}
                        //    onChange={(e) => handleChange("goal", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>
                    <div style={{ marginBottom: "15px" }}>
                        <label>Súly</label><br />
                        <input
                            readOnly 
                            type="number"
                            value={userData.weight}
                        //    onChange={(e) => handleChange("weight", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Magasság</label><br />
                        <input
                            readOnly
                            type="number"
                            value={userData.height}
                          //  onChange={(e) => handleChange("height", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Életkor</label><br />
                        <input
                            readOnly
                            type="number"
                            value={userData.age}
                          //  onChange={(e) => handleChange("age", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Nem</label><br />
                        <input
                            readOnly
                            type="text"
                            value={userData.gender}
                         //   onChange={(e) => handleChange("gender", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>
                    <div style={{ marginBottom: "15px" }}>
                        <label>Edzések hetente</label><br />
                        <input
                            readOnly
                            type="number"
                            value={userData.trainingsperweek}
                         //   onChange={(e) => handleChange("trainingsperweek", e.target.value)}
                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>
                    <Button type="button" onClick={handleCalculateAndFetchMacros} disabled={loading} color="black">
                        {loading ? "Számítás..." : "Kiszámítás"}
                    </Button>


                    {result !== null && <p style={{ marginTop: "15px" }}>Eredmény: {result} kcal</p>}
                    {error && <p style={{ color: "red" }}>⚠️ {error}</p>}
                </form>
            </div>

            <div
                style={{
                    maxWidth: "400px",
                    padding: "20px",
                    border: "1px solid #ccc",
                    borderRadius: "10px",
                    flex: "1",
                    backgroundColor: "#212529",
                    color: "#ffffffff",
                }}
            >
                <h2 style={{ textAlign: "center", marginBottom: "20px" }}>Tápanyagok</h2>

                <form>
                    <div style={{ marginBottom: "15px" }}>
                        <label>Fehérje</label><br />
                        <input
                            readOnly
                            type="number"
                            value={userData.protein ?? 0}

                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Zsír</label><br />
                        <input
                            readOnly
                            type="number"
                            value={userData.fat ?? 0}

                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                    <div style={{ marginBottom: "15px" }}>
                        <label>Szénhidrát</label><br />
                        <input
                            readOnly
                            type="number"
                            value={userData.carbs ?? 0}

                            style={{ width: "100%", padding: "8px", borderRadius: "5px", border: "1px solid #ccc" }}
                        />
                    </div>

                </form>
            </div>
        </div>


    );
};

export default Kcalcalculator;
