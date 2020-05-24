<template>
    
    <div class="settings">
        <div class="settings-element">
            <ButtonAsText text="Кол-во аптек"/>
            <p-radio class="radio-button" v-model="settings.limitedPharmaciesCount" v-bind:value="false" name="pharmacies-count"><ButtonAsText text="Любое"/></p-radio>
            <p-radio class="radio-button" v-model="settings.limitedPharmaciesCount" v-bind:value="true" name="pharmacies-count"><ButtonAsText text="Задать:"/></p-radio>
            <div class="slider-wrap" >
                <VueSlider class="slider" v-model="settings.pharmaciesCount" v-bind:min="1" :max="5" :interval="1"
                           :tooltip-formatter="'{value} шт.'"
                           :processStyle="{backgroundColor: settings.limitedPharmaciesCount ? '#777' : '#eee'}"
                           :railStyle="{backgroundColor: '#eee'}"
                           :dotStyle="{border: '2px solid #777'}" :disabled="!settings.limitedPharmaciesCount" />
            </div>
        </div>
        <div class="settings-element">
            <ButtonAsText text="Радиус поиска"/>
            <p-radio class="radio-button" v-model="settings.useSearchRadius" v-bind:value="false" name="search-radius"><ButtonAsText text="Любой"/></p-radio>
            <p-radio class="radio-button" v-model="settings.useSearchRadius" v-bind:value="true" name="search-radius"><ButtonAsText text="Задать:"/></p-radio>
            <div class="slider-wrap" >
                <VueSlider class="slider" v-model="settings.searchRadius" v-bind:min="100" :max="5000" :interval="100"
                           :tooltip-formatter="'{value} метров'"
                           :processStyle="{backgroundColor: settings.useSearchRadius ? '#777' : '#eee'}"
                           :railStyle="{backgroundColor: '#eee'}"
                           :dotStyle="{border: '2px solid #777'}" :disabled="!settings.useSearchRadius" />
            </div>
        </div>
    </div>
</template>

<script>
    import PrettyRadio from 'pretty-checkbox-vue/radio';
    import VueSlider from 'vue-slider-component'
    import ButtonAsText from './ButtonAsText'
    import 'vue-slider-component/theme/antd.css'
    import GeoLocationProvider from '../utils/GeoLocationProvider'
    export default {
        name: "Settings",
        components: {
            VueSlider,
            ButtonAsText,
            'p-radio': PrettyRadio,
        },
        props: {
            settings: Object
        },
        mounted: function () {
            this.$nextTick(function () {
                GeoLocationProvider
                    .provide()
                    .then(result => {
                        console.log(result);
                        this.settings.latitude = result[0];
                        this.settings.longitude = result[1];
                    });
                
            })
        }
    }
</script>

<style scoped>
    .settings {
        grid-column: 1/2;
        display: grid;
        grid-template-rows: repeat(2, 1fr);
        
    }

    .settings-element {
        grid-template-columns: repeat(5, 1fr);
        display: grid;
    }

    .slider-wrap {
        grid-column: 4/6;
        display: flex;
    }

    .slider {
        margin: auto;
        width: 100% !important;
        padding-right: 15px !important;
        padding-left: 15px !important;
    }
    
    .radio-button {
        display: grid;
        grid-template-columns: 1fr 3fr;
    }

    
</style>
<style>
    .radio-button > input {
        margin: 5px;
    }
</style>