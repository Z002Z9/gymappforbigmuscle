import {Center, Image, Paper, Text} from "@mantine/core";

interface AuthContainerInterface {
    children: JSX.Element;
}

const AuthContainer = ({children}: AuthContainerInterface) => {
    return <div className="auth-container">
        <Center><Image src="/logo.png" alt="img" w={400} mt={50}/></Center>
        <Center>
            <Paper radius="md" p="xl" maw={600} m={10} bg="#212529" withBorder={false}>
                <Text size="lg" fw={500} c="#F1F3F5">
                    Üdvözlünk a Gymapp for big muscle felületén!
                </Text>
                
                {children}
            </Paper>
        </Center>
    </div>
}

export default AuthContainer;