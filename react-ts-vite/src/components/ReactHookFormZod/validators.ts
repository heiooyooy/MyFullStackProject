import { z } from "zod";

export const signUpSchema = z
  .object({
    username: z
      .string()
      .min(3, { error: "Username should be at least 3 characters." })
      .max(20, { error: "Username should not be longer than 20 characters" }),
    email: z.email({ error: "Please enter valid email." }),
    password: z
      .string()
      .min(8, { error: "Password should be at least 8 characters" }),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    error: `Password don't match`,
    path: ["confirmPassword"],
  });

export type SignUpFormValues = z.infer<typeof signUpSchema>;
