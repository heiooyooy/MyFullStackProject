import { useOptimistic, useRef, useState } from "react";
import { Button } from "../shared/UI/button";
import Input from "@mui/material/Input";
import TextField from "@mui/material/TextField";

const deliverMessage = async (message: string) => {
  await new Promise((res) => setTimeout(res, 1000));
  if (message.includes("error")) {
    throw new Error("Failed to send message");
  }
  return message;
};
const Message = ({
  message,
  isSending,
}: {
  message: string;
  isSending: boolean;
}) => {
  return (
    <li>
      {message}
      {isSending && <small style={{ marginLeft: "8px" }}>(Sending...)</small>}
    </li>
  );
};

const UseOptimisticTest = () => {
  const [messages, setMessages] = useState([
    { text: "hello everyone", sending: false },
  ]);

  const formRef = useRef<HTMLFormElement>(null);

  const [optimisticMessages, setOptimisticMessages] = useOptimistic(
    messages,
    (currentMessages, newOptimisticMessage: string) => [
      ...currentMessages,
      { text: newOptimisticMessage, sending: true },
    ]
  );

  const formAction = async (formData: FormData) => {
    const newMessage = formData.get("message") as string;
    formRef.current?.reset();
    setOptimisticMessages(newMessage);
    try {
      const messageResult = await deliverMessage(newMessage);
      setMessages((prev) => [...prev, { text: messageResult, sending: false }]);
      console.log(messageResult);
    } catch (error) {
      console.log(error);
    }
  };
  return (
    <div>
      <h2>Chatting Room</h2>
      <ul>
        {optimisticMessages.map((message, index) => (
          <Message
            key={index}
            message={message.text}
            isSending={message.sending}
          ></Message>
        ))}
      </ul>
      <form action={formAction} ref={formRef}>
        <TextField type="text" name="message" placeholder="Input message..." />
        <Button type="submit">Send</Button>
      </form>
    </div>
  );
};

export default UseOptimisticTest;
