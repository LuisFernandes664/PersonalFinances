import { AuthProvider } from './auth-provider.interface';

export abstract class BaseAuthService implements AuthProvider {
  abstract signIn(): Promise<string>;
  abstract signOut(): Promise<void>;

  protected log(message: string): void {
    console.log(`BaseAuthService: ${message}`);
  }
}
