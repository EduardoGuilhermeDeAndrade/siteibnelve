import { Component, inject } from '@angular/core';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './dashboard.html'
})
export class Dashboard {
  protected readonly authService = inject(AuthService);
}
