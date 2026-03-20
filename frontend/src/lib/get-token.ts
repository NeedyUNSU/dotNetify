import { cookies } from 'next/headers';

export const getToken = async () => (await cookies()).get('.AspNetCore.Identity.Application');
