import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { SelectionMenuComponent } from './selection-menu/selection-menu.component';

import { GuessTheCountryComponent } from './guess-the-country/guess-the-country.component';

import { PictionaryComponent } from './pictionary/pictionary.component';
import { WordListComponent } from './pictionary/word-list/word-list.component';
import { ImageListComponent } from './pictionary/image-list/image-list.component';

import { CathphraseComponent } from './cathphrase/cathphrase.component';

import { SpeedTextingComponent } from './speed-texting/speed-texting.component';


import { TimerComponent } from './core-components/timer/timer.component';
import { ScoreComponent } from './core-components/score/score.component';

//Services
import { AnimalService } from './services/animal-service.service';

@NgModule({
  declarations: [
    AppComponent,
    SelectionMenuComponent,
    GuessTheCountryComponent,
    PictionaryComponent,
    CathphraseComponent,
    SpeedTextingComponent,
    WordListComponent,
    ImageListComponent,
    TimerComponent,
    ScoreComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule
  ],
  providers: [AnimalService],
  bootstrap: [AppComponent]
})
export class AppModule { }
