import { type NextRequest, NextResponse } from 'next/server';

const publicRoutes = ['/login', '/register'];

export const config = {
	matcher: ['/((?!api|_next|_vercel|static|favicon.ico|sitemap.xml).*)']
};

export async function middleware(request: NextRequest) {
	const { pathname } = request.nextUrl;

	if (publicRoutes.includes(pathname)) {
		return NextResponse.next();
	}

	const token = request.cookies.get('.AspNetCore.Identity.Application')?.value;

	if (!token) {
		const loginUrl = new URL('/login', request.url);
		return NextResponse.redirect(loginUrl);
	}
}
