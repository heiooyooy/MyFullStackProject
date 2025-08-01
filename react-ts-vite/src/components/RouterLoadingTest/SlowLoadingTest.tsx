import type { QueryClient } from "@tanstack/react-query";
import { useLoaderData } from "react-router-dom";

interface Contact {
  id: number;
  name: string;
  email: string;
}

const contacts: Contact[] = [
  { id: 1, name: "张三", email: "zhangsan@example.com" },
  { id: 2, name: "李四", email: "lisi@example.com" },
];

const getContactAPI = async (): Promise<Contact[]> => {
  console.log("api: getting contacts....");
  await new Promise((res) => setTimeout(res, 1000));
  return contacts;
};

export const clientLoader = (queryClient: QueryClient) => async () => {
  console.log("Slow loading component Loader: starts loading...");
  const contactList = await getContactAPI();
  console.log("Slow loading component Loader: data loaded, start rendering...");
  return { contactList };
};

const SlowLoadingTest = () => {
  const { contactList } = useLoaderData();

  return (
    <div>
      <h1>My Contacts</h1>
      <ul>
        {contactList.map((contact: Contact) => (
          <li key={contact.id}>
            {contact.name} ({contact.email}){" "}
          </li>
        ))}
      </ul>
      <hr />
      <h3>添加新联系人</h3>
    </div>
  );
};

export default SlowLoadingTest;
