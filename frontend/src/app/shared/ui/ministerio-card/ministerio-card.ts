import { Component, Input } from '@angular/core';

import { Ministerio } from '../../data/ministerios.service';

@Component({
  selector: 'app-ministerio-card',
  templateUrl: './ministerio-card.html',
  styleUrl: './ministerio-card.css'
})
export class MinisterioCard {
  @Input({ required: true }) ministerio!: Ministerio;
}
