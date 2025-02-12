export interface AuthProvider {
  signIn(): Promise<string>;
  signOut(): Promise<void>;
}
