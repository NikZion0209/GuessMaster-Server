import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { SelectionMenuComponent } from './selection-menu/selection-menu.component';

import { FlagWhizComponent } from './flag-whiz/flag-whiz.component';
import { ImageSingleComponent } from './core-components/image-single/image-single.component';

import { WordSnapComponent } from './word-snap/word-snap.component';
import { WordListComponent } from './core-components/word-list/word-list.component';
import { ImageListComponent } from './core-components/image-list/image-list.component';

import { WordyWondersComponent } from './wordy-wonders/wordy-wonders.component';

import { TextBlitzComponent } from './text-blitz/text-blitz.component';


import { TimerComponent } from './core-components/timer/timer.component';
import { ScoreComponent } from './core-components/score/score.component';
import { WordOptionsComponent } from './core-components/word-options/word-options.component';

@NgModule({
  declarations: [
    AppComponent,
    SelectionMenuComponent,
    FlagWhizComponent,
    WordSnapComponent,
    TextBlitzComponent,
    WordListComponent,
    ImageListComponent,
    TimerComponent,
    ScoreComponent,
    WordyWondersComponent,
    ImageSingleComponent,
    WordOptionsComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
