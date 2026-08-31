import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-admin-layout',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css'
})
export class AdminLayout {
  protected readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected async sair(): Promise<void> {
    await this.authService.logout();
    await this.router.navigateByUrl('/admin/login');
  }
}
