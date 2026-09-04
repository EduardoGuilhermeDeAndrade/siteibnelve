import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';

import { EventoAgenda } from '../../data/eventos.service';

@Component({
  selector: 'app-event-card',
  imports: [DatePipe],
  templateUrl: './event-card.html',
  styleUrl: './event-card.css'
})
export class EventCard {
  @Input({ required: true }) evento!: EventoAgenda;
}
