import {
    Stack,
    TextInput,
    PasswordInput,
    Group,
    Button,
    
    
} from "@mantine/core";
import {useForm} from "@mantine/form";
import {useNavigate} from "react-router-dom";
import AuthContainer from "../components/AuthContainer.tsx";
import useAuth from "../hooks/useAuth.tsx";

const Login = () => {
    const {login} = useAuth();
    const navigate = useNavigate();

    const form = useForm({
        initialValues: {
            email: '',
            password: '',
        },

        validate: {
            email: (val: string) => (/^\S+@\S+$/.test(val) ? null : 'Érvénytelen e-mail cím'),
            password: (val: string) => (val.length <= 6 ? 'A jelszónak 6 karakter hosszúnak kell lennie.' : null),
        },
    });


    const submit = () => {
        login(form.values.email, form.values.password)
    }

    return <AuthContainer>        
        <div>
            
            <form onSubmit={form.onSubmit(submit)}>
                <Stack>
                    <TextInput
                        required
                        label="E-mail cím"
                        placeholder="nev@gmail.com"
                        key={form.key('email')}
                        radius="md"
                        {...form.getInputProps('email')}
                        labelProps={{ style: { color: '#F1F3F5' } }}
                        mt="xl"
                        mb="md"
                    />

                    <PasswordInput
                        required
                        label="Jelszó"
                        placeholder="Jelszo"
                        key={form.key('password')}
                        radius="md"
                        {...form.getInputProps('password')}
                        labelProps={{ style: { color: '#F1F3F5' } }}
                        mb="sm"
                    />
                </Stack>

                <Group mt="xl" gap="xl" justify="center">                    
                    <Button 
                        component="button" type="button" onClick={() => navigate('/registrate')} color="#ffffffff" radius="xl" c="black">
                        Regisztráció
                    </Button>
                    <Button type="submit" radius="xl" color="#121214" c="white">
                        Bejelentkezés
                    </Button>
                </Group>
                
            </form>
            
        </div>
    </AuthContainer>
}

export default Login;