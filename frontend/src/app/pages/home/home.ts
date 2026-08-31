import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { LOCAIS_MOCK } from '../../shared/data/locais.mock';
import { MINISTERIOS_MOCK } from '../../shared/data/ministerios.mock';
import { EventCard } from '../../shared/ui/event-card/event-card';
import { MinisterioCard } from '../../shared/ui/ministerio-card/ministerio-card';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';
import { proximosEventosPublicos } from '../../shared/data/eventos.mock';

@Component({
  selector: 'app-home',
  imports: [RouterLink, EventCard, MinisterioCard, SectionHeading],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  protected readonly proximosEventos = proximosEventosPublicos(3);
  protected readonly ministerios = MINISTERIOS_MOCK;
  protected readonly locais = LOCAIS_MOCK;
}
