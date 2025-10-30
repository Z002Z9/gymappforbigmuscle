import {
    Stack,
    TextInput,
    PasswordInput,
    Group,
    Button,
    Anchor,
    Text,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useNavigate } from "react-router-dom";
import AuthContainer from "../components/AuthContainer.tsx";
import axios from "axios";
import { useState } from "react";


const Registrate = () => {

    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null);

    const form = useForm({
        initialValues: {
            name: "",
            email: "",
            password: "",
        },

        validate: {
            email: (val: string) =>
                /^\S+@\S+$/.test(val) ? null : "Érvénytelen e-mail cím",
            password: (val: string) =>
                val.length < 6 ? "A jelszónak legalább 6 karakter hosszúnak kell lennie." : null,
        },
    });

    const handleSubmit = async (values: typeof form.values) => {
        setError(null);

        try {
            // Ellenőrizzük, hogy az email létezik-e
            const res = await axios.get(`https://localhost:7226/api/user/EmailExists?email=${encodeURIComponent(values.email)}`
            );

            if (res.data === true) {
                setError("Ez az email már foglalt!");
                return;
            }
            await axios.post("https://localhost:7226/api/user/Registration", {
                name: values.name,
                email: values.email,
                password: values.password,
                roleId: 2, //csak usert tudjunk regisztrálni, admint pedig majd swaggeren 
            });

            
            navigate("/login");
        } catch (err: any) {
            console.error(err);
            setError("Hiba történt a regisztráció során. Kérlek, próbáld újra.");
        }
    };

    return (
        <AuthContainer title="Regisztráció">
            <form onSubmit={form.onSubmit(handleSubmit)}>
                <Stack>
                    <TextInput
                        required
                        label="Név"
                        placeholder="Név"
                        {...form.getInputProps("name")}
                    />

                    <TextInput
                        required
                        label="E-mail"
                        placeholder="pelda@domain.hu"
                        {...form.getInputProps("email")}
                    />

                    <PasswordInput
                        required
                        label="Jelszó"
                        placeholder="********"
                        {...form.getInputProps("password")}
                    />

                    {error && <Text color="red">{error}</Text>}

                    <Group position="apart" mt="md">
                        <Anchor size="sm" onClick={() => navigate("/login")}>
                            Vissza a bejelentkezéshez
                        </Anchor>
                        <Button type="submit">Regisztráció</Button>
                    </Group>
                </Stack>
            </form>
        </AuthContainer>
    );
};

export default Registrate;
