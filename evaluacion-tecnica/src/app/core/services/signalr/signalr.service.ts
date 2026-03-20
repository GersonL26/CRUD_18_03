import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private connection: signalR.HubConnection | null = null;
  readonly conectado = signal(false);

  async conectar(sesionId: string): Promise<void> {
    if (this.connection) await this.desconectar();

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.connection.onreconnected(() => this.conectado.set(true));
    this.connection.onclose(() => this.conectado.set(false));

    await this.connection.start();
    this.conectado.set(true);
    await this.connection.invoke('UnirseASesion', sesionId);
  }

  on<T>(evento: string, callback: (data: T) => void): void {
    this.connection?.on(evento, callback);
  }

  off(evento: string): void {
    this.connection?.off(evento);
  }

  async desconectar(): Promise<void> {
    if (this.connection) {
      try { await this.connection.stop(); } catch {}
      this.connection = null;
      this.conectado.set(false);
    }
  }
}
