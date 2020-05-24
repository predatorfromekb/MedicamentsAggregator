import Vue from 'vue';
import App from './App.vue';
import Result from "./components/Result";
import Index from "./components/Index";
import VueRouter from "vue-router";

Vue.config.productionTip = false;
Vue.config.devtools = true;

const routes = [
  { path: '/', component: Index},
  { path: '/result', name:"result", component: Result, props: true}
];

const router = new VueRouter({
  routes,
  mode: 'history'
});

Vue.use(VueRouter);

new Vue({
  router,
  el: '#app',
  render: h => h(App),
});
