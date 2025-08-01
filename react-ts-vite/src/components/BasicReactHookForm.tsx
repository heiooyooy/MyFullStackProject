import { zodResolver } from "@hookform/resolvers/zod";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import { useReducer, useRef } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import z from "zod";

const schema = z.object({
  email: z.email(),
  password: z.string().min(8),
});

type FormFields = z.infer<typeof schema>;

// type FromFields = {
//   email: string;
//   password: string;
// };

export default function BasicReactHookForm() {
  const inputEl = useRef<HTMLInputElement>(null);

  const onButtonClick = () => {
    // `current` 指向已挂载到 DOM 上的 input 元素
    if (inputEl) inputEl.current?.focus();
  };

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<FormFields>({
    defaultValues: {
      email: "",
      password: "",
    },
    resolver: zodResolver(schema),
  });

  const onSubmit: SubmitHandler<FormFields> = async (data) => {
    try {
      await new Promise((resolve) => setTimeout(resolve, 1000));
      throw new Error("");
      console.log(data);
    } catch {
      setError("root", {
        message: "This emial is already taken",
      });
    }
  };
  return (
    <Box>
      <TextField inputRef={inputEl} type="text" />
      <Button onClick={onButtonClick}>Focus the input</Button>

      <div>
        <form className="tutorial gap-2" onSubmit={handleSubmit(onSubmit)}>
          <TextField
            size="small"
            {...register("email")}
            type="text"
            placeholder="Email"
          />
          {errors.email && <div>{errors.email.message}</div>}
          <TextField
            size="small"
            {...register("password")}
            type="password"
            placeholder="Password"
          />
          {errors.password && <div>{errors.password.message}</div>}
          <Button variant="contained" disabled={isSubmitting} type="submit">
            {isSubmitting ? "Submitting..." : "Submit"}
          </Button>

          {errors.root && <div>{errors.root.message}</div>}
        </form>
      </div>
    </Box>
  );
}
